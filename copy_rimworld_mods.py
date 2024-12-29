import os
import shutil

# List of folders to update
folders = [
    '1103809207',
    '1127530465',
    '1321849735',
    '2023407836',
    '2362707956',
    '747397788'
]

# Source directory for updates
source_dir = 'D:/rimworld-mods-source/'
# Target directory where updates will be applied
target_dir = 'D:/rimworld-mods-git/'

# Debugging function
def debug_log(message):
    print(f"[DEBUG] {message}")

# Function to copy and overwrite existing files only
def copy_if_exists(src, dst):
    if os.path.exists(dst):
        debug_log(f"Updating file: {dst}")
        shutil.copy2(src, dst)
    else:
        debug_log(f"File does not exist, skipping: {dst}")

# Process each folder
for folder in folders:
    src_path = os.path.join(source_dir, folder)
    dst_path = os.path.join(target_dir, folder)

    debug_log(f"Checking folder: {folder}")

    # Skip if source folder doesn't exist
    if not os.path.exists(src_path):
        debug_log(f"Source folder missing, skipping: {src_path}")
        continue

    # Ensure target folder exists
    os.makedirs(dst_path, exist_ok=True)

    # Walk through source files
    for root, dirs, files in os.walk(src_path):
        rel_path = os.path.relpath(root, src_path)
        target_root = os.path.join(dst_path, rel_path)
        os.makedirs(target_root, exist_ok=True)

        for file in files:
            src_file = os.path.join(root, file)
            dst_file = os.path.join(target_root, file)

            copy_if_exists(src_file, dst_file)

debug_log("Update process completed.")
