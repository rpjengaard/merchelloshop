Function CopyDeployFolder([string]$source, [bool]$useMIR, [string[]]$exclude) {
    $sourceDirectory = $PSScriptRoot + "\Dev\web\" + $source
    $targetDirectory = $PSScriptRoot.Replace("Solutions", "Websites") + "\" + $source
    $MIR = ""
    $ExcludeFiles = $exclude

    if($useMIR) {
        $MIR = "/MIR"
    } else {
        $MIR = "/E"
    }
    
    robocopy $sourceDirectory $targetDirectory $MIR /NFL /NDL /NJH /XF $ExcludeFiles[0] $ExcludeFiles[1] $ExcludeFiles[2] $ExcludeFiles[3] $ExcludeFiles[4] $ExcludeFiles[5] $ExcludeFiles[6]
}

Function CopyDeployItem([string]$source) {
    $sourceDirectory = $PSScriptRoot + "\Dev\web\" + $source
    $targetDirectory = $PSScriptRoot.Replace("Solutions", "Websites") + "\" + $source

    Copy-Item $sourceDirectory $targetDirectory
}

CopyDeployFolder -source:'App_Browsers' -useMIR:1 -exclude:@('*.txt');
CopyDeployFolder -source:'App_Code' -useMIR:1 -exclude:@('*.txt');
CopyDeployFolder -source:'App_Data' -useMIR:0 -exclude:@('umbraco.config','access.config');
CopyDeployFolder -source:'App_Plugins' -useMIR:1 -exclude:@('*.txt');
CopyDeployFolder -source:'bin' -useMIR:1 -exclude:@('*.pdb', '*.xml');
CopyDeployFolder -source:'config' -useMIR:1 -exclude:@('*.txt');
CopyDeployFolder -source:'css' -useMIR:1 -exclude:@('*.txt');
CopyDeployFolder -source:'img' -useMIR:1 -exclude:@('*.txt');
CopyDeployFolder -source:'media' -useMIR:0 -exclude:@('*.txt');
CopyDeployFolder -source:'scripts' -useMIR:1 -exclude:@('*.txt');
CopyDeployFolder -source:'svgs' -useMIR:1 -exclude:@('*.txt');
CopyDeployFolder -source:'umbraco' -useMIR:1 -exclude:@('*.txt');
CopyDeployFolder -source:'umbraco_client' -useMIR:1 -exclude:@('*.txt');
CopyDeployFolder -source:'Views' -useMIR:1 -exclude:@('*.txt');
CopyDeployFolder -source:'node' -useMIR:1 -exclude:@('*.aaa');
CopyDeployItem('default.aspx');
CopyDeployItem('Global.asax');
CopyDeployItem('package.json');
CopyDeployItem('Web.config');
CopyDeployItem('web.config');