#!python3
import subprocess, argparse, os, zipfile, shutil, glob

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
    
    # Package zip 
    print("\n\nRunning Step: Package")
    packageDir('pnyx.cmd/.out/', 'cmd.zip')
    
def buildNuget():
    return



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





