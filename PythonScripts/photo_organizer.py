import os
from datetime import datetime
import exifread

file_path = 'C:\\stary laptop v2\\zdjęcia\\DCIM\\Camera\\kwiatki.jpg'
def get_creation_date(file_path):
    with open(file_path, 'rb') as f:
        tags = exifread.process_file(f)
        date_taken = tags.get('EXIF DateTimeOriginal')
        if date_taken:
            return datetime.strptime(str(date_taken), '%Y:%m:%d %H:%M:%S')


def get_creation_path(file_path):
    creation_date = get_creation_date(file_path)
    if creation_date:
        year = creation_date.strftime('%Y')
        month = creation_date.strftime('%m')
        dest_dir = os.path.join(year, month)
        return dest_dir.replace("\\","/")