import sys
import json

def main(args):
    if len(args) == 0:
        print("Received no files!")
        return
    
    # Load files paths to list
    jsonFile = open(args)
    files : list[str] = json.load(jsonFile)
    
    # Way to return data from script to C#
    for file in files:
        print(file)

# C:\Temp\basfk.temp
if __name__ == "__main__":
    main(sys.argv[1])