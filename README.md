# 快速上手 ANTLR 與 .NET 開發環境設定

有時候我們需要自訂 DSL (Domain Specific Language) 領域特定語言，就需要自訂格式、語法，並自製編譯器去解析這份語言。目前最為普遍的作法大概就是
[ANTLR](https://www.antlr.org/) 工具了。這個工具主要由 Java 開發而成，但可以自動產生 C# 程式碼，讓你用 C# 去解析這份語言，甚至可以用這份語言產生另一份語言，這也是 DSL 的另一種常見情境。

### 簡介 ANTLR (ANother Tool for Language Recognition)

[ANTLR](https://www.antlr.org/) 是一套威力強大的 **Parser Generator** (解析一份 DSL 語言的程式碼產生器)，可以用來讀取、解析、執行、轉譯一份**結構化的**文字或二進位檔案。這套工具通常用來打造一個程式語言、工具或框架。

使用 ANTLR 大致的流程如下：

1. 撰寫一份 Grammar 文法定義檔

    副檔名若為 `.g4` 就代表格式為 ANTLR v4 支援的文法定義檔

2. 使用 ANTLR 產生一個 Parser 程式碼

    目前 ANTLR v4 支援 Java, C#, Python, JavaScript, Go, C++, Swift, PHP, Dart 等語言

3. 使用 Parser 去解析出 AST (Abstract Syntax Tree) 抽象語法樹

    你可以透過這份 AST 取遍瀝所有的程式內容

### 安裝 ANTLR 工具

ANTLR 是一套 Java 命令列工具，你可以直接從[官網下載](https://www.antlr.org/download/) `.jar` 檔即可執行。

如果你還沒有安裝 Java Runtime 可以參考以下命令快速安裝：

```ps1
choco install zulu8 maven -y

# 設定 JAVA_HOME 環境變數
$env:JAVA_HOME = 'C:\Program Files\Zulu\zulu-8'
[System.Environment]::SetEnvironmentVariable('JAVA_HOME','C:\Program Files\Zulu\zulu-8',[System.EnvironmentVariableTarget]::Machine)
```

如果你是 Windows 用戶，可以直接透過 [Chocolatey](https://community.chocolatey.org/packages/antlr4) 使用以下命令進行安裝。如果你沒有先安裝 Java Runtime 的話，依然無法執行 ANTLR 工具。

```ps1
choco install antlr4 -y
```

> 完整的安裝步驟可以參考 [Getting Started with ANTLR v4](https://github.com/antlr/antlr4/blob/master/doc/getting-started.md) 文件說明。

### 體驗最基本的 ANTLR 開發範例

以下我用 .NET 6 進行範例解說，所以我們現來建立基本練習環境：

1. 確定 .NET SDK 版本

    ```ps1
    C:\>dotnet --version
    6.0.100-preview.5.21302.13
    ```

2. 建立 Console 專案範本

    ```ps1
    mkdir FirstAntlr4Example && cd FirstAntlr4Example
    dotnet new console
    code .
    ```

3. 請加入一個 `Properties/AssemblyInfo.cs` 檔案

    ```cs
    [assembly: System.CLSCompliant(true)]
    ```

    > 如果不加入這段組件屬性宣告，之後在編譯的時候會出現一堆 [CS3021](https://docs.microsoft.com/en-us/dotnet/csharp/misc/cs3021?WT.mc_id=DT-MVP-4015686) 編譯器警告訊息。

4. 加入 `.gitignore` 檔案 ([`dotnet tool install -g dotnet-ignore`](https://github.com/Arasz/dotnet-ignore))

    ```ps1
    dotnet ignore get -n visualstudio
    ```

5. 加入 Antlr 相關 NuGet 套件

    ```ps1
    dotnet add package Antlr4.Runtime.Standard
    ```

接下來就來體驗 ANTLR 的開發過程

1. 撰寫一份 Grammar 文法定義檔 (`Hello.g4`)

    ```antlr4
    // Define a grammar called Hello
    grammar Hello;
    r  : 'hello' ID ;         // match keyword hello followed by an identifier
    ID : [a-z]+ ;             // match lower-case identifiers
    WS : [ \t\r\n]+ -> skip ; // skip spaces, tabs, newlines
    ```

    這份文法定義檔，主要宣告我們的 `Hello` 語言必須以 `hello` 開頭，中間要有一個以上的空白字元，然後接著 `[a-z]+` (一個以上的小寫英文字母)。最後的 `WS` 則是宣告空白字元的格式，多餘的空白都會自動忽略。

2. 使用 ANTLR 產生一組 Parser 程式碼

    以下命令會透過 `Hello.g4` 產生好幾個 `*.cs` 檔案在 `Hello` 目錄下：

    ```ps1
    antlr4.exe -Dlanguage=CSharp Hello.g4 -listener -visitor -o Hello
    ```

    這個過程會產生幾個重要的檔案：

    ```txt
    HelloLexer.cs
    HelloParser.cs
    HelloListener.cs
    HelloBaseListener.cs
    HelloVisitor.cs       (要加上 -visitor 參數才會產生這個檔)
    HelloBaseVisitor.cs   (要加上 -visitor 參數才會產生這個檔)
    ```

    建置專案

    ```ps1
    dotnet build
    ```

3. 使用 Parser 去解析出 AST (Abstract Syntax Tree) 抽象語法樹

    我們先用 `HelloLexer` 分析現有內容，把文字 Tokenize (將我們的語言切成一段一段的)，然後再利用 `HelloParser` 去解析這份內容，最後透過 `IParseTree` 的 `ToStringTree()` 方法將解析後的內容全部輸出！

    ```cs
    using System;
    using Antlr4.Runtime;
    using Antlr4.Runtime.Tree;

    namespace FirstAntlr4Example
    {
        class Program
        {
            static void Main(string[] args)
            {
                String input = "hello will";

                var stream = new AntlrInputStream(input);
                var lexer = new HelloLexer(stream);
                var tokens = new CommonTokenStream(lexer);
                var parser = new HelloParser(tokens);
                var compileUnit = parser.r();

                Console.WriteLine(compileUnit.ToStringTree());
            }
        }
    }
    ```

    重新建置並執行：

    ```ps1
    dotnet run
    ```

    此時你會得到以下結果：

    ```txt
    ([] hello will)
    ```

    如果你將 `input` 修改為 `hello will.`，再執行一次就會顯示以下結果：

    ```txt
    line 1:10 token recognition error at: '.'
    ([] hello will)
    ```

### 相關連結

- [ANTLR](https://www.antlr.org/)
- [Getting Started with ANTLR v4](https://github.com/antlr/antlr4/blob/master/doc/getting-started.md)
- [Runtime Libraries and Code Generation Targets](https://github.com/antlr/antlr4/blob/master/doc/targets.md)
  - [C♯](https://github.com/antlr/antlr4/blob/master/doc/csharp-target.md)
  - [Git repo page for ANTLR C# runtime.](https://github.com/antlr/antlr4/tree/master/runtime/CSharp)
- [NuGet Gallery | Antlr4.CodeGenerator 4.6.6](https://www.nuget.org/packages/Antlr4.CodeGenerator/)
  - [C# target for ANTLR 4](https://github.com/tunnelvisionlabs/antlr4cs) (這套有支援 MSBuild 但版本有點舊)
