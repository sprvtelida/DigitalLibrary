#pragma checksum "C:\Users\sapar\Desktop\shared\DigitalLibrary\DigitalLibrary.API\Views\Shared\_Layout.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "882578853fe149be9e133cdf330a1542e42deabd"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Shared__Layout), @"mvc.1.0.view", @"/Views/Shared/_Layout.cshtml")]
namespace AspNetCore
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#nullable restore
#line 1 "C:\Users\sapar\Desktop\shared\DigitalLibrary\DigitalLibrary.API\Views\_ViewImports.cshtml"
using DigitalLibrary.Models.AccountModels;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "C:\Users\sapar\Desktop\shared\DigitalLibrary\DigitalLibrary.API\Views\_ViewImports.cshtml"
using Microsoft.AspNetCore.Mvc.Localization;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"882578853fe149be9e133cdf330a1542e42deabd", @"/Views/Shared/_Layout.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"e7c81ba89d360fae9ac7a6256f1c042c6b6956c9", @"/Views/_ViewImports.cshtml")]
    public class Views_Shared__Layout : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
    {
        #line hidden
        #pragma warning disable 0649
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperExecutionContext __tagHelperExecutionContext;
        #pragma warning restore 0649
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner __tagHelperRunner = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner();
        #pragma warning disable 0169
        private string __tagHelperStringValueBuffer;
        #pragma warning restore 0169
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __backed__tagHelperScopeManager = null;
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __tagHelperScopeManager
        {
            get
            {
                if (__backed__tagHelperScopeManager == null)
                {
                    __backed__tagHelperScopeManager = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager(StartTagHelperWritingScope, EndTagHelperWritingScope);
                }
                return __backed__tagHelperScopeManager;
            }
        }
        private global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.HeadTagHelper __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_HeadTagHelper;
        private global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.BodyTagHelper __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_BodyTagHelper;
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral("<!DOCTYPE html>\r\n<html>\r\n");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("head", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "882578853fe149be9e133cdf330a1542e42deabd3329", async() => {
                WriteLiteral("\r\n    <title>");
#nullable restore
#line 4 "C:\Users\sapar\Desktop\shared\DigitalLibrary\DigitalLibrary.API\Views\Shared\_Layout.cshtml"
      Write(ViewData["Title"]);

#line default
#line hidden
#nullable disable
                WriteLiteral("</title>\r\n    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">\r\n");
            }
            );
            __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_HeadTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.HeadTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_HeadTagHelper);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\r\n");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("body", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "882578853fe149be9e133cdf330a1542e42deabd4640", async() => {
                WriteLiteral("\r\n\r\n    <div class=\"form-wrapper\">\r\n        ");
#nullable restore
#line 10 "C:\Users\sapar\Desktop\shared\DigitalLibrary\DigitalLibrary.API\Views\Shared\_Layout.cshtml"
   Write(RenderBody());

#line default
#line hidden
#nullable disable
                WriteLiteral("\r\n    </div>\r\n\r\n");
            }
            );
            __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_BodyTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.BodyTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_BodyTagHelper);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\r\n</html>\r\n\r\n<style type=\"text/css\">\r\n    ");
            WriteLiteral(@"@font-face {
        font-family: 'Montserrat', sans-serif;
        src: url('https://fonts.googleapis.com/css2?family=Montserrat:wght@200;300;400;500;700&display=swap');
    }

    html, body {
    	position: relative;
    	width: 100%;
    	height: 100%;
    	font-size: 18px;
    }

    body {
    	color: black;
    	background: white;
    	margin: 0;
    	padding: 8px;
    	box-sizing: border-box;
    	font-family: -apple-system, BlinkMacSystemFont, ""Segoe UI"", Roboto, Oxygen-Sans, Ubuntu, Cantarell, ""Helvetica Neue"", sans-serif;
    }

    a {
    	color: rgb(0,100,200);
    	text-decoration: none;
    }

    a:hover {
    	text-decoration: underline;
    }

    a:visited {
    	color: rgb(0,80,160);
    }

    label {
    	display: block;
    }

    input, button, select, textarea {
    	font-family: inherit;
    	font-size: inherit;
    	padding: 0.4em;
    	margin: 0 0 0.5em 0;
    	box-sizing: border-box;
    	border: 1px solid #ccc;
    	border-radius: 2px");
            WriteLiteral(@";
    }

    input:disabled {
    	color: #ccc;
    }

    input[type=""range""] {
    	height: 0;
    }

    button {
    	color: #333;
    	background-color: #f4f4f4;
    	outline: none;
    }

    button:disabled {
    	color: #999;
    }

    button:not(:disabled):active {
    	background-color: #ddd;
    }

    button:focus {
    	border-color: #666;
    }

    :root {
        --text-color: black;
        --btn-color: #93B44D;
    }

	.form-wrapper {
		display: flex;
		align-items: center;
		flex-direction: column;
		font-family: sofia-pro, sans-serif;
		text-align: center;
	}

    form {
        max-width: 500px;
		text-align: left;
    }

    .field {
        width: 100%;
        position: relative;
        border-bottom: 2px dashed var(--text-color);
        margin: 4rem auto 1rem;
    }

    .label {
        color: #666;
        font-size: 1.2rem;
    }

    .input {
        outline: none;
        border: none;
        overflow: hidden;
   ");
            WriteLiteral(@"     margin: 0;
        width: 100%;
        padding: 0.25rem 0;
        background: none;
        color: black;
        font-size: 1.2em;
        font-weight: bold;
        transition: border 500ms;
      }

      .field::after {
		content: """";
		position: relative;
		display: block;
		height: 4px;
		width: 100%;
		background: #d16dff;
		transform: scaleX(0);
		transform-origin: 0%;
		transition: transform 500ms ease;
		top: 2px;
      }

      .field:focus-within {
      	border-color: transparent;
      }

      .field:focus-within::after {
      	transform: scaleX(1);
      }

      .label {
      	z-index: -1;
      	position: absolute;
      	transform: translateY(-2rem);
      	transform-origin: 0%;
      	transition: transform 400ms;
      }

      .field:focus-within .label,
      .input:not(:placeholder-shown) + .label{
      	transform: scale(0.8) translateY(-5rem);
      }

      span {
      	color: orangered;
      }

      .link {
      	display:");
            WriteLiteral(@" block;
      	text-align: center;
      	margin-top: 5px;
      }

      .button {
      	background: var(--btn-color);
      	width: 100%;
      	padding: 0.7rem;
      	margin: 10px 0;
      	font-weight: bold;
      	color: yellow;
      }

      .button:hover {
      	background: yellow;
      	color: var(--btn-color);
      }

      .button:active {
      	transform: scale(0.98);
      }


</style>
");
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<dynamic> Html { get; private set; }
    }
}
#pragma warning restore 1591
