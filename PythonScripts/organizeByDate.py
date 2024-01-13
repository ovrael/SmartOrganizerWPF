import sys
import random
import tempfile
import locale
import exifread
from datetime import datetime
import os
import time

class OrganizeFile:
    def __init__(self, path, organizedPath=""):
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
        # print(file.path)
        level0: str = random.choice(randomDirectoriesLevel0)
        level1: str = random.choice(randomDirectoriesLevel1[level0])
        file.organizedPath = f"{level0}/{level1}"
        if(random.random() < 0.7): # 1 for 5 files will not be organized fully, only at level 0
            file.organizedPath = f"{level0}/Other"

    return organizedFiles

def get_creation_date(file_path):
    try:
        with open(file_path, 'rb') as f:
            tags = exifread.process_file(f)
            date_taken = tags.get('EXIF DateTimeOriginal')
            if date_taken:
                return datetime.strptime(str(date_taken), '%Y:%m:%d %H:%M:%S')
    except Exception:
        pass
    return None

def organize_by_date(organizedFiles: list[OrganizeFile]) -> list[OrganizeFile]:

    for file in organizedFiles:
        
        statinfo = os.stat(file.path)
        statinfo.st_ctime
        # creation_timestamp = os.path.getctime(rf'{file.path}')
        creation_datetime = datetime.fromtimestamp(statinfo.st_ctime)
        creation_year = creation_datetime.year
        creation_month = creation_datetime.month

        if creation_year != None:
            file.organizedPath = f"{creation_year}"
        else:
            file.organizedPath = f"Other"
        
        if creation_month != None:
            file.organizedPath += f"/{creation_month}"
        else:
            file.organizedPath += f"/Other"
        
        # # try:

        # except Exception:
        #     file.organizedPath = f"Other"
        #     pass
        
    return organizedFiles

def main(filePath):
    # Something went wrong with passing arguments
    if len(filePath) == 0:
        print("Received no files!")
        return

    # Load files paths to OrganizeFile list
    organizedFiles: list[OrganizeFile] = []
    with open(filePath, mode='r', encoding='utf-8-sig') as file:
        organizedFiles = [OrganizeFile(line.strip(), "") for line in file]

    # Randomize organized directory for given files
    organizedFiles = organize_by_date(organizedFiles)

    # Write organized files (path?organizedDirectory) to temporary file
    path: str = saveOrganizedFiles(organizedFiles)

    # Return temporary file path to C# via console output
    print(path)


# C:\Temp\basfk.temp
if __name__ == "__main__":
    main(sys.argv[1])
