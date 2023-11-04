import sys
import json
import tempfile

def writePathsToFile(filePaths : list[str]) -> str:
    with tempfile.NamedTemporaryFile(mode='w', delete=False) as tmpFile:
        for path in filePaths:
            tmpFile.write(path + '\n')
        tmpFilePath :str= tmpFile.name
    return tmpFilePath

def main(filePath):
    if len(filePath) == 0:
        print("Received no files!")
        return
    
    # Load files paths to list
    with open(filePath) as file:
        lines : list[str] = [line.rstrip() for line in file]
        
    path :str = writePathsToFile(lines)
    
    # Way to return data from script to C#    
    print(path)

# C:\Temp\basfk.temp
if __name__ == "__main__":
    main(sys.argv[1])