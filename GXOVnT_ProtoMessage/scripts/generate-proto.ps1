
# Nanopd is used to generate the c++ file proto source files
[string]$global:nanopd_git_url = "https://github.com/nanopb/nanopb/archive/refs/heads/master.zip"
# Standard protoc is used to generate the c# proto source files
[string]$global:protoc_git_url = "https://github.com/protocolbuffers/protobuf/releases/download/v26.1/protoc-26.1-win64.zip"

# Input directories
[string]$global:root_directory = "C:\src\Trevor\GXOVnT"
[string]$global:root_input_directory = ""
[string]$global:messages_input_directory = ""

# Generator Directories
[string]$global:root_generator_directory = ""
[string]$global:root_generator_protoc_directory = ""
[string]$global:root_generator_nanopd_directory = ""

# Generator Files
[string]$global:protoc_generator = ""
[string]$global:nanopd_generator = ""

# Source Output Directories
[string]$global:src_output_csharp = ""
[string]$global:src_output_cpp = ""

# Final Copy directories
[string]$global:final_output_cpp_headers = ""
[string]$global:final_output_cpp_source = ""


function SetupInputAndOutputDirectories() {
    $global:root_input_directory = "$global:root_directory\GXOVnT_ProtoMessage"
    $global:root_generator_directory = "$global:root_input_directory\generators"
    $global:root_generator_protoc_directory = "$global:root_generator_directory\protoc"
    $global:root_generator_nanopd_directory = "$global:root_generator_directory\nanopd"

    $global:protoc_generator = "$global:root_generator_protoc_directory\bin\protoc.exe"
    $global:nanopd_generator = "$global:root_generator_nanopd_directory\nanopb-master\generator\protoc-gen-nanopb.bat"
    $global:nanopd_extra = "$global:root_generator_nanopd_directory\nanopb-master"
    
    $global:messages_input_directory = "$global:root_input_directory\proto"
    $global:src_output_csharp = "$global:root_input_directory\src\c#"
    $global:src_output_cpp = "$global:root_input_directory\src\c++"

    $global:final_output_cpp_headers = "$global:root_directory\GXOVnT_Shared\include\messages"
    $global:final_output_cpp_source = "$global:root_directory\GXOVnT_Shared\src\messages"
    
}

function PrepareGeneratorsDirectory() {
    #=============== Custom Proto Generators directories ===============
    if (!(Test-Path -Path $global:root_generator_directory)) {
        New-Item -ItemType Directory -Force -Path $global:root_generator_directory
    }
    if (!(Test-Path -Path $global:root_generator_protoc_directory)) {
        New-Item -ItemType Directory -Force -Path $global:root_generator_protoc_directory
    }
    if (!(Test-Path -Path $global:root_generator_nanopd_directory)) {
        New-Item -ItemType Directory -Force -Path $global:root_generator_nanopd_directory
    }

    #=============== Geneated c# files output directory ===============
    if (!(Test-Path -Path $global:src_output_csharp )) {
        New-Item -ItemType Directory -Force -Path $global:src_output_csharp 
    }
    Get-ChildItem -Path $global:src_output_csharp *.* -File -Recurse | foreach { $_.Delete()}

    #=============== Geneated c++ files output directory ===============
    if (!(Test-Path -Path $global:src_output_cpp )) {
        New-Item -ItemType Directory -Force -Path $global:src_output_cpp 
    }
    Get-ChildItem -Path $global:src_output_cpp *.* -File -Recurse | foreach { $_.Delete()}
    
    # #=============== Final output headers directory ===============
    if (!(Test-Path -Path $global:final_output_cpp_headers )) {
        New-Item -ItemType Directory -Force -Path $global:final_output_cpp_headers 
    }
    Get-ChildItem -Path $global:final_output_cpp_headers *.* -File | foreach { $_.Delete()}

    # #=============== Final output source directory ===============
    if (!(Test-Path -Path $global:final_output_cpp_source )) {
        New-Item -ItemType Directory -Force -Path $global:final_output_cpp_source 
    }
    Get-ChildItem -Path $global:final_output_cpp_source *.* -File | foreach { $_.Delete()}
}

function DownloadAndExtract_protoc() {
    If ((Get-ChildItem -Path $global:root_generator_protoc_directory -Force | Measure-Object).Count -eq 0) {
        
        $zipFile = "$global:root_generator_protoc_directory\protoc.zip"
        Invoke-WebRequest $global:protoc_git_url -OutFile $zipFile
        Expand-Archive $zipFile -DestinationPath $global:root_generator_protoc_directory
    }
}

function DownloadAndExtract_nanopd() {
    If ((Get-ChildItem -Path $global:root_generator_nanopd_directory -Force | Measure-Object).Count -eq 0) {
        
        $zipFile = "$global:root_generator_nanopd_directory\nanopd.zip"
        Invoke-WebRequest $global:nanopd_git_url -OutFile $zipFile
        Expand-Archive $zipFile -DestinationPath $global:root_generator_nanopd_directory
    }
}

function PrepareEnvironment() {
    pip install protobuf grpcio-tools
}

function GenerateFiles_CSharp() {

    Get-ChildItem $global:messages_input_directory -Filter *.proto | Foreach-Object {
        $command = "$global:protoc_generator --proto_path=$global:messages_input_directory --csharp_out=$global:src_output_csharp $_"
        Invoke-Expression $command
    }
}

function GenerateFiles_CPP() {
    Get-ChildItem $global:messages_input_directory -Filter *.proto | Foreach-Object {
        $command = "$global:protoc_generator --plugin=protoc-gen-nanopb=$global:nanopd_generator --proto_path=$global:messages_input_directory --nanopb_out=$global:src_output_cpp $_"
        Invoke-Expression $command
    }
}

function CopyCPPFilesToShared() {

    Get-ChildItem $global:src_output_cpp -Filter *.h | Foreach-Object {
        
        $destinationFileName = $global:final_output_cpp_headers + "\" + $_.NameString
        Copy-Item $_ -Destination $destinationFileName -force 
        (Get-Content $destinationFileName) -replace '<pb.h>', """pb.h""" | Set-Content $destinationFileName
    }

    Get-ChildItem $global:src_output_cpp -Filter *.c | Foreach-Object {

        $oldIncludePath = """" + $_.NameString.Replace(".c", ".h") + """"
        $newIncludePath = """messages/" + $_.NameString.Replace(".c", ".h") + """"
        $destinationFileName = $global:final_output_cpp_source + "\" + $_.NameString
        # Copy the c file
        Copy-Item $_ -Destination $destinationFileName -force 
        # fix the content
        (Get-Content $destinationFileName) -replace $oldIncludePath, $newIncludePath | Set-Content $destinationFileName
    }
    
    Copy-Item "$global:nanopd_extra\pb.h" -Destination "$global:final_output_cpp_headers\pb.h" -force 
    Copy-Item "$global:nanopd_extra\pb_common.c" -Destination "$global:final_output_cpp_headers\pb_common.c" -force 
    Copy-Item "$global:nanopd_extra\pb_common.h" -Destination "$global:final_output_cpp_headers\pb_common.h" -force 
    Copy-Item "$global:nanopd_extra\pb_decode.c" -Destination "$global:final_output_cpp_headers\pb_decode.c" -force 
    Copy-Item "$global:nanopd_extra\pb_decode.h" -Destination "$global:final_output_cpp_headers\pb_decode.h" -force 
    Copy-Item "$global:nanopd_extra\pb_encode.h" -Destination "$global:final_output_cpp_headers\pb_encode.h" -force 
    Copy-Item "$global:nanopd_extra\pb_encode.c" -Destination "$global:final_output_cpp_headers\pb_encode.c" -force 

}

SetupInputAndOutputDirectories
PrepareGeneratorsDirectory
DownloadAndExtract_protoc
DownloadAndExtract_nanopd
PrepareEnvironment
GenerateFiles_CSharp
GenerateFiles_CPP
CopyCPPFilesToShared