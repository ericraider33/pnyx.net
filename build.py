#!/usr/bin/env python3
# Refernces: 
# https://docs.microsoft.com/en-us/nuget/quickstart/create-and-publish-a-package-using-the-dotnet-cli
# https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet?tabs=netcore21
#
# To release nuget:
# - From IDE:
#       - Update pnyx.net NuGet properties version
#       - Update pnyx.net Assembly version
#       - Update pncs.cmd Assembly version
#       - Update pnyx.cmd Assembly version
# - build.py nuget
# - Using web, upload result on page: https://www.nuget.org/packages/manage/upload
#
# To release CMD:
# - build.py cmd
# - Using web, upload resulting zip on page: https://s3.console.aws.amazon.com/s3/buckets/bto-web-content/pnyx/cmd/?region=us-east-1&tab=overview
#
import subprocess, argparse, os, zipfile, shutil, glob, re, platform, pathlib, sys

def minPython(tuple):
    if sys.version_info < tuple:
        sys.exit("Python %s.%s or later is required.\n" % tuple)
minPython((3,7))

parser = argparse.ArgumentParser(description='Builds pnyx.net')
parser.add_argument('target', help='Target of either: cmd, nuget, localInstall')

args = parser.parse_args()

def resetDirectory(path):
    if os.path.exists(path):
        shutil.rmtree(path)
    if os.path.exists(path) == False:
        os.mkdir(path)
    print("Verified Dependency: resetDirectory=" + path)

def verifyDependencyDotNet():
    subprocess.run(['dotnet','--info'], check=True, capture_output=True)
    print("Verified Dependency: DotNet")    

def verifyDependencyNuget():
    subprocess.run(['nuget','help'], check=True, capture_output=True)
    print("Verified Dependency: Nuget")    

def copyDir(source, dest):
    for toCopy in glob.glob(source + "/*"):
        shutil.copy(toCopy, dest)        
    
def packageDir(path, name):
    with zipfile.ZipFile(os.path.join(path, name), 'w', zipfile.ZIP_DEFLATED) as cmdZip:
        for root, dirs, files in os.walk(path):
            for file in files:
                if file != name:
                    file = os.path.join(root, file)
                    arcname = file.replace(path,'')
                    cmdZip.write(file, arcname=arcname)

def dos2unix(path):                    
    text = open(path).read()
    open(path, "w", newline='\n').write(text)                    
    print('Converted newlines to unix for file: ', path)
    
def checkRootAccess():
    if os.getuid() != 0:
        print("You must run as either root or sudo")
        exit(1)    
            
def buildCmd():
    pathOut = os.path.abspath(".out")
    verifyDependencyDotNet()    
    resetDirectory(pathOut)

    # Restores nuget packages
    print("\n\nRunning Step: Restore")
    subprocess.run(['dotnet','restore'], check=True)
       
    # Cleans build
    print("\n\nRunning Step: Clean")
    subprocess.run(['dotnet','clean','--configuration','Release'], check=True)
              
    # Publish build
    print("\n\nRunning Step: Publish")
    subprocess.run(['dotnet','publish','--self-contained','true','--configuration','Release','--output',pathOut+'/lib','pnyx.cmd/pnyx.cmd.csproj'], check=True)
    subprocess.run(['dotnet','publish','--self-contained','true','--configuration','Release','--output',pathOut+'/lib','pncs.cmd/pncs.cmd.csproj'], check=True)
    
    # Copys deployment files
    copyDir('deploy',pathOut)
    
    # Reads version number
    version = subprocess.check_output(['dotnet',pathOut+'/lib/pncs.cmd.dll','-v'], stderr=subprocess.STDOUT).decode()
    version = version.strip()
    version = re.sub(".*[ ]", "", version)
    print('Packaging CMD version:')
    
    # Assures bash files are unix
    for newlineFix in glob.glob(pathOut+'/*.bsh'):
        dos2unix(newlineFix)    
    
    # Package zip 
    packageName = 'pnyx.cmd-{0}.zip'.format(version)
    print("\n\nRunning Step: Package", packageName)
    packageDir(pathOut, packageName)
    
    # Print AWS url
    print("\nUpload cmd.zip file to URL: https://s3.console.aws.amazon.com/s3/buckets/bto-web-content/pnyx/cmd/?region=us-east-1&tab=overview")
    
def buildNuget():
    verifyDependencyDotNet()    
    resetDirectory('pnyx.net/.out')
       
    # Cleans build
    print("\n\nRunning Step: Clean")
    subprocess.run(['dotnet','clean','--configuration','Release'], check=True)

    print("\n\nRunning Step: Restore")
    subprocess.run(['dotnet','restore','pnyx.net/pnyx.net.csproj'], check=True)
	
    # Pack build
    print("\n\nRunning Step: Pack")
    subprocess.run(['dotnet','pack','--output','.out/lib','pnyx.net/pnyx.net.csproj'], check=True)
    
    # Prints nuget URL
    print("\nUpload 'nupkg' file to URL: https://www.nuget.org/packages/manage/upload")


def findCmdPackage():
    build_zip_listing = glob.glob('.out/pnyx.cmd-*.zip')
    if len(build_zip_listing) == 0:
        print('Could not locate pnyx.cmd.zip file')
        exit(1)
    if len(build_zip_listing) != 1:
        print('Found',len(build_zip_listing),'pnyx.cmd.zip files.  Only one package can be installed.')
        exit(1)
    return build_zip_listing[0]
    
def localInstall():
    if platform.system() != 'Linux':
        raise Exception("Only run on Linux")
    
    checkRootAccess()

    buildZip = findCmdPackage()
    print('Installing build locally from file',buildZip)
    
    if not os.path.exists('/opt'):
        print('Making directory: /opt')
        os.mkdir('/opt')
    
    print('Resetting directory /opt/pnyx')
    resetDirectory('/opt/pnyx')
    
    with zipfile.ZipFile(buildZip) as zf:
        zf.extractall('/opt/pnyx')

    print('Setting file permissions')
    pathObj = pathlib.Path('/opt/pnyx')
    for pathObj in pathObj.glob('**/*.bsh'):
        pathText = str(pathObj.absolute())
        os.chmod(pathText, 0o755)

    if not os.path.exists('/usr/local/bin/pnyx'):
        print('Create symbolic link for: pnyx')
        subprocess.run(['ln','-s','/opt/pnyx/pnyx.bsh','/usr/local/bin/pnyx'], check=True)

    if not os.path.exists('/usr/local/bin/pncs'):
        print('Create symbolic link for: pncs')
        subprocess.run(['ln','-s','/opt/pnyx/pncs.bsh','/usr/local/bin/pncs'], check=True)
    
    print('Install complete')
    
    print('Verifing results:')

    print("\tpnyx -i '[readString: hello world]'") 
    hello = subprocess.check_output(['pnyx','-i','[readString: hello from pnyx]'], stderr=subprocess.STDOUT).decode()
    print("\t\t",hello)

    print("\tpncs -i 'readString(\"hello world\")'") 
    hello = subprocess.check_output(['pncs','-i','readString("hello from pncs")'], stderr=subprocess.STDOUT).decode()
    print("\t\t",hello)
    print()
    

##########################
## Runs build
#########################  
    
# Downloads package from build server
if args.target == 'cmd':
    buildCmd()
elif args.target == 'nuget':
    buildNuget()
elif args.target == 'localInstall':
    localInstall()
else:
    raise Exception('Unknown target ' + args.target)
    
print("SUCCESS: build '{0}' complete".format(args.target))






