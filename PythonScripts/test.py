import sys
import random
import tempfile
import locale


class OrganizeFile:
    def __init__(self, path, organizedPath):
        self.path = path
        self.organizedPath = organizedPath
        
    def createSavePath(self) -> str:
        return f'{self.path}?{self.organizedPath}\n'


def saveOrganizedFiles(organizedFiles: list[OrganizeFile]) -> str:
    with tempfile.NamedTemporaryFile(mode='w', delete=False, encoding='utf-8') as tmpFile:
        for file in organizedFiles:
            tmpFile.write(file.createSavePath())
        tmpFilePath: str = tmpFile.name
    return tmpFilePath


def test(organizedFiles: list[OrganizeFile]) -> list[OrganizeFile]:

    randomDirectoriesLevel0 = ["Krajobrazy", "Rośliny",
                               "Zwierzęta", "Technologia",
                               "Dźwięki", "Jedzenie", "Książki"]
    randomDirectoriesLevel1 = {
        "Krajobrazy": ["Góry", "Morze", "Las", "Równiny"],
        "Rośliny": ["Tropikalne", "Doniczkowe", "Ogrodowe"],
        "Zwierzęta": ["Psy", "Koty", "Krowy", "Papugi", "Ryby"],
        "Technologia": ["Komputery", "Neony", "Smartfony"],
        "Dźwięki": ["Natura", "Instrumenalne", "Elektroniczne"],
        "Jedzenie": ["Fast food", "Warzywa", "Owoce", "Ryby"],
        "Książki": ["Fantasy", "Naukowe", "Romanse", "Historyczne"]
    }

    for file in organizedFiles:
        level0: str = random.choice(randomDirectoriesLevel0)
        level1: str = random.choice(randomDirectoriesLevel1[level0])
        file.organizedPath = f"{level0}/{level1}"
        if(random.random() < 0.7): # 1 for 5 files will not be organized fully, only at level 0
            file.organizedPath = f"{level0}/Other"

    return organizedFiles


def main(filePath):
    # Something went wrong with passing arguments
    if len(filePath) == 0:
        print("Received no files!")
        return

    # Load files paths to OrganizeFile list
    organizedFiles: list[OrganizeFile] = []
    with open(filePath) as file:
        organizedFiles = [OrganizeFile(line.strip(), "") for line in file]

    # Randomize organized directory for given files
    organizedFiles = test(organizedFiles)

    # Write organized files (path?organizedDirectory) to temporary file
    path: str = saveOrganizedFiles(organizedFiles)

    # Return temporary file path to C# via console output
    print(path)


# C:\Temp\basfk.temp
if __name__ == "__main__":
    main(sys.argv[1])
