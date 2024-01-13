import os
import shutil
from datetime import datetime
import exifread

source_directory = 'C:\\stary laptop v2\\zdjęcia\\DCIM\\Camera'
destination_directory = '\\organizer'


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


for root, dirs, files in os.walk(source_directory):
    for file in files:
        print(file)
        file_path = os.path.join(root, file)

        creation_date = get_creation_date(file_path)
        if creation_date:
            year = creation_date.strftime('%Y')
            month = creation_date.strftime('%m')

            source_directory_name = os.path.basename(root)

            new_filename = f"{source_directory_name}_{file}"

            dest_dir = os.path.join(destination_directory, year, month)
            os.makedirs(dest_dir, exist_ok=True)

            shutil.copy(file_path, os.path.join(dest_dir, new_filename))
        else:
            rel_path = os.path.relpath(file_path, source_directory)
            dest_file_path = os.path.join(destination_directory, 'nieskategoryzowane', rel_path)
            os.makedirs(os.path.dirname(dest_file_path), exist_ok=True)
            shutil.copy(file_path, dest_file_path)

print("Organizacja plików została ukończona!")