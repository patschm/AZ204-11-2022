Connect-AzAccount
New-AzResourceGroup -Name "Test2" -Location "westeurope"
New-AzResourceGroupDeployment -ResourceGroupName "Test2" -Mode Complete -TemplateFile .\template.json #-TemplateParameterFile .\parameters.json