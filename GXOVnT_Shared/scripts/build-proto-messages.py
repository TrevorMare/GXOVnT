Import("env")
import os
import shutil
import hashlib
import pathlib
import shlex
import subprocess

import SCons.Action
from platformio import fs

python_exe = env.subst("$PYTHONEXE")
try:
    import protobuf
except ImportError:
    print("[nanopb] Installing Protocol Buffers dependencies");

    # We need to specify protobuf version. In other case got next (on Ubuntu 20.04):
    # Requirement already satisfied: protobuf in /usr/lib/python3/dist-packages (3.6.1)
    subprocess.run([python_exe, '-m', 'pip', 'install', "protobuf>=3.19.1"])

try:
    import grpc_tools.protoc
except ImportError:
    print("[nanopb] Installing gRPC dependencies");
    subprocess.run([python_exe, '-m', 'pip', 'install', "grpcio-tools>=1.43.0"])

print("---------------------------------")
print("Starting build of proto messages")
print("---------------------------------")

# Get the root project directory, it should in theory always be one leve up from the project directory
rootProject_dir = os.path.dirname(env.subst("$PROJECT_DIR"))
# Now get the library directory and the message prototype directories
messageProtoTypeDirSource = os.path.join(rootProject_dir, "GXOVnT_ProtoMessage", "proto")
messageProtoTypeDirDest = os.path.join(rootProject_dir, "GXOVnT_Shared", "include", "messages", "generate")
srcDirDest = os.path.join(rootProject_dir, "GXOVnT_Shared", "include", "messages")
nanopb_generator = os.path.join(rootProject_dir, "GXOVnT_Shared", "scripts", "nanopb_generator.py")

# If the source directory does not exist, quit the script
if not os.path.exists(messageProtoTypeDirSource):
    print("Proto message source does not exist at: " + messageProtoTypeDirSource)
    exit()

# Create the destination directory if it does not exist in the library location
if not os.path.exists(messageProtoTypeDirDest):
    os.mkdir(messageProtoTypeDirDest)

# Clear out the files in the destination library directory
for root, dirs, files in os.walk(messageProtoTypeDirDest):
    for f in files:
        os.unlink(os.path.join(root, f))
    for d in dirs:
        shutil.rmtree(os.path.join(root, d))

# Copy the proto and option files from the source to the destination
for root, dirs, files in os.walk(messageProtoTypeDirSource):
    for f in files:
        shutil.copy(os.path.join(root, f), os.path.join(messageProtoTypeDirDest, f))



nanopb_options = []
nanopb_options.extend(["--output-dir", srcDirDest])

for root, dirs, files in os.walk(messageProtoTypeDirSource):
    for f in files:
        proto_file_basename = os.path.basename(f)
        cmd = [python_exe, nanopb_generator] + nanopb_options + [proto_file_basename]
        action = SCons.Action.CommandAction(cmd)
        result = env.Execute(action)