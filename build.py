#!python3
# Refernces: 
# https://docs.microsoft.com/en-us/nuget/quickstart/create-and-publish-a-package-using-the-dotnet-cli
# https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet?tabs=netcore21
#
# To release nuget:
# - From IDE:
#       - Update pnyx.net NuGet properties version
#       - Update pnyx.net Assembly version
#       - Update pnyx.cmd Assembly version
# - build.py nuget
# - Using web, upload result on page: https://www.nuget.org/packages/manage/upload
#
# To release CMD:
# - build.py cmd
# - Using web, upload resulting zip on page: https://s3.console.aws.amazon.com/s3/buckets/bto-web-content/pnyx/cmd/?region=us-east-1&tab=overview
#
import subprocess, argparse, os, zipfile, shutil, glob, re

parser = argparse.ArgumentParser(description='Builds pnyx.net')
parser.add_argument('target', help='Target of either: cmd or nuget')

args = parser.parse_args()

def resetDirectory(path):
    if os.path.exists(path):
        shutil.rmtree(path)
    else:
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
            
def buildCmd():
    verifyDependencyDotNet()    
    resetDirectory('pnyx.cmd/.out')
       
    # Cleans build
    print("\n\nRunning Step: Clean")
    subprocess.run(['dotnet','clean','--configuration','Release'], check=True)
       
    # Publish build
    print("\n\nRunning Step: Publish")
    subprocess.run(['dotnet','publish','--configuration','Release','--output','.out/lib','pnyx.cmd/pnyx.cmd.csproj'], check=True)
    
    # Copys deployment files
    copyDir('deploy','pnyx.cmd/.out/')
    
    # Reads version number
    version = subprocess.check_output(['dotnet','pnyx.cmd/.out/lib/pnyx.cmd.dll','-v'], stderr=subprocess.STDOUT).decode()
    version = version.strip()
    version = re.sub(".*[ ]", "", version)
    print('Packaging CMD version:')
    
    # Assures bash files are unix
    for newlineFix in glob.glob('pnyx.cmd/.out/*.bsh'):
        dos2unix(newlineFix)    
    
    # Package zip 
    packageName = 'pnyx.cmd-{0}.zip'.format(version)
    print("\n\nRunning Step: Package", packageName)
    packageDir('pnyx.cmd/.out/', packageName)
    
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



##########################
## Runs build
#########################  
    
# Downloads package from build server
if args.target == 'cmd':
    buildCmd()
elif args.target == 'nuget':
    buildNuget()
else:
    raise Exception('Unknown target ' + args.target)
    
print("SUCCESS: build '{0}' complete".format(args.target))






