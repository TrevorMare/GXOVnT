Import("env")
import os
import subprocess, sys

# Get the root project directory, it should in theory always be one level up from the project directory
rootDirectory = os.path.dirname(env.subst("$PROJECT_DIR"))
scriptPath = os.path.join(rootDirectory, "GXOVnT_ProtoMessage", "scripts", "generate-proto.ps1")

p = subprocess.Popen(["powershell.exe", scriptPath + " -rootDirectory " + rootDirectory ], stdout=sys.stdout)
p.communicate
