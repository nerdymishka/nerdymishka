Import-Module "$PSScriptRoot/../NerdyMishka-BuildUtil.psm1" -Force 


InModuleScope -ModuleName "NerdyMishka-BuildUtil" {
    
    Describe "Read-DotEnv" {
        $Env:TEST_VALUE = $null 
        $Env:TEST_DOUBLE_ONE_LINE = $null 
        $env:TEST_SINGLE_ONE_LINE = $null
        $Env:TEST_DOUBLE = $null 
        $Env:TEST_EMPTY = $null 
        $Env:TEST_JSON = $null


        Read-DotEnv "$PsScriptRoot/.env"

        $multi = "woah
I have multiline values
yea!"
        $json = "{
    `"name`": `"value`",
    `"array`": [
        `"x`",
        `"y`",
        `"z`"
    ]
}"  

        $Env:TEST_VALUE | Should Be "x"
        $n = [Environment]::NewLine
        $Env:TEST_DOUBLE_ONE_LINE | Should Be "my multiline ${n}text"    
        $Env:TEST_SINGLE_ONE_LINE | Should Be "single"
        $Env:TEST_DOUBLE | Should Be $multi
        $Env:TEST_JSON | Should Be $Json
        $Env:TEST_EMPTY | Should Be $Null
    }
}