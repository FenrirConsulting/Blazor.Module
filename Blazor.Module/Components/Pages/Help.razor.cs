using Microsoft.AspNetCore.Components;
using Microsoft.Graph.Models;
using Blazor.RCL.Infrastructure.Services.Interfaces;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Text;
using Blazor.RCL.Application.Interfaces;
using Blazor.RCL.Application.Common.Configuration.Interfaces;
using System.Text.Json;
using DocumentFormat.OpenXml;

namespace Blazor.Module.Components.Pages
{
    /// <summary>
    /// Represents the help page component for the EnableDisable tool.
    /// </summary>
    public partial class Help : ComponentBase, IDisposable
    {
        #region Injected Services
        [Inject] private ISharePointService? _sharePointService { get; set; }
        [Inject] private IToolsConfigurationRepository? _toolsRepository { get; set; }
        [Inject] private IAppConfiguration? _appConfiguration { get; set; }
        #endregion

        #region Private Fields
        private DriveItem? _document;
        private byte[]? _documentContent;
        private bool _isLoading = true;
        private string _documentName = string.Empty;
        private string _libraryName = string.Empty;
        #endregion

        #region Lifecycle Methods
        /// <summary>
        /// Initializes the component and retrieves the help document from SharePoint.
        /// </summary>
        protected override async Task OnInitializedAsync()
        {
            try
            {
                _isLoading = true;
                await LoadHelpDocument();
            }
            finally
            {
                _isLoading = false;
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Loads the help document from SharePoint.
        /// </summary>
        private async Task LoadHelpDocument()
        {
            var helpDocumentConfig = await _toolsRepository!.GetValueAsync(_appConfiguration!.AppName, "HelpDocument");

            if (string.IsNullOrEmpty(helpDocumentConfig))
            {
                // Handle the case where the configuration is not found
                return;
            }

            try
            {
                var helpDocumentInfo = JsonSerializer.Deserialize<HelpDocumentInfo>(helpDocumentConfig);

                if (helpDocumentInfo == null)
                {
                    // Handle the case where the JSON is invalid
                    return;
                }

                _libraryName = helpDocumentInfo.DocumentLibrary;
                _documentName = helpDocumentInfo.Document;

                var documents = await _sharePointService!.GetDocumentsAsync(_libraryName);
                _document = documents.FirstOrDefault(d => d.Name!.Equals(_documentName, StringComparison.OrdinalIgnoreCase));

                if (_document != null)
                {
                    using var stream = await _sharePointService.DownloadDocumentAsync(_document.Id!);
                    if (stream != null)
                    {
                        using var memoryStream = new MemoryStream();
                        await stream.CopyToAsync(memoryStream);
                        _documentContent = memoryStream.ToArray();
                    }
                }
            }
            catch (JsonException ex)
            {
                // Log the error and handle invalid JSON
                Console.WriteLine($"Error parsing JSON: {ex.Message}");
                // You might want to set an error message or state here
            }
        }

        /// <summary>
        /// Converts the DOCX content to HTML, preserving Word document styling.
        /// </summary>
        /// <returns>A string containing the HTML representation of the document with preserved styling.</returns>
        private string ConvertDocxToHtml()
        {
            if (_documentContent == null) return string.Empty;

            using (MemoryStream ms = new MemoryStream(_documentContent))
            using (WordprocessingDocument doc = WordprocessingDocument.Open(ms, false))
            {
                var mainPart = doc.MainDocumentPart;
                var stylesPart = mainPart!.StyleDefinitionsPart;
                var numberingPart = mainPart.NumberingDefinitionsPart;

                StringBuilder html = new StringBuilder();
                html.Append("<style>");
                html.Append(GenerateCSS(stylesPart!, numberingPart!));
                html.Append("</style>");

                var body = mainPart.Document.Body;
                foreach (var element in body!.ChildElements)
                {
                    html.Append(ConvertElementToHtml(element, stylesPart!, numberingPart!));
                }

                return html.ToString();
            }
        }

        /// <summary>
        /// Generates CSS based on the Word document's style definitions.
        /// </summary>
        /// <param name="stylesPart">The styles part of the Word document.</param>
        /// <param name="numberingPart">The numbering part of the Word document.</param>
        /// <returns>A string containing CSS rules based on Word styles.</returns>
        private string GenerateCSS(StyleDefinitionsPart stylesPart, NumberingDefinitionsPart numberingPart)
        {
            StringBuilder css = new StringBuilder();

            if (stylesPart != null && stylesPart.Styles != null)
            {
                foreach (var style in stylesPart.Styles.Elements<Style>())
                {
                    string? styleId = style.StyleId ?? string.Empty;
                    var styleRunProperties = style.StyleRunProperties;
                    var styleParagraphProperties = style.StyleParagraphProperties;

                    css.Append($".{styleId} {{");

                    if (styleRunProperties != null)
                    {
                        if (styleRunProperties.FontSize != null && int.TryParse(styleRunProperties.FontSize.Val, out int fontSize))
                            css.Append($"font-size: {fontSize / 2}pt;");
                        if (styleRunProperties.Color != null)
                            css.Append($"color: #{styleRunProperties.Color.Val};");
                        if (styleRunProperties.Bold != null)
                            css.Append("font-weight: bold;");
                        if (styleRunProperties.Italic != null)
                            css.Append("font-style: italic;");
                    }

                    if (styleParagraphProperties?.SpacingBetweenLines != null)
                    {
                        if (styleParagraphProperties.SpacingBetweenLines.After != null &&
                            int.TryParse(styleParagraphProperties.SpacingBetweenLines.After, out int afterSpacing))
                            css.Append($"margin-bottom: {afterSpacing / 20}pt;");
                        if (styleParagraphProperties.SpacingBetweenLines.Before != null &&
                            int.TryParse(styleParagraphProperties.SpacingBetweenLines.Before, out int beforeSpacing))
                            css.Append($"margin-top: {beforeSpacing / 20}pt;");
                        if (styleParagraphProperties.SpacingBetweenLines.Line != null &&
                            int.TryParse(styleParagraphProperties.SpacingBetweenLines.Line, out int lineSpacing))
                            css.Append($"line-height: {lineSpacing / 240}%;");
                    }

                    css.Append("}");
                }
            }

            // Add CSS for numbering if needed
            if (numberingPart != null)
            {
                // Implement numbering CSS generation here
            }

            return css.ToString();
        }

        /// <summary>
        /// Converts a Word document element to its HTML representation.
        /// </summary>
        /// <param name="element">The OpenXml element to convert.</param>
        /// <param name="stylesPart">The styles part of the Word document.</param>
        /// <param name="numberingPart">The numbering part of the Word document.</param>
        /// <returns>A string containing the HTML representation of the element.</returns>
        private string ConvertElementToHtml(OpenXmlElement element, StyleDefinitionsPart stylesPart, NumberingDefinitionsPart numberingPart)
        {
            StringBuilder html = new StringBuilder();

            if (element is Paragraph p)
            {
                string styleClass = p.ParagraphProperties?.ParagraphStyleId?.Val?.Value ?? "Normal";
                html.Append($"<p class='{styleClass}'>");
                foreach (var run in p.Elements<DocumentFormat.OpenXml.Wordprocessing.Run>())
                {
                    html.Append(ConvertRunToHtml(run));
                }
                html.Append("</p>");
            }
            else if (element is Table table)
            {
                html.Append("<table border='1'>");
                foreach (var row in table.Elements<TableRow>())
                {
                    html.Append("<tr>");
                    foreach (var cell in row.Elements<TableCell>())
                    {
                        html.Append("<td>");
                        foreach (var cellElement in cell.ChildElements)
                        {
                            html.Append(ConvertElementToHtml(cellElement, stylesPart, numberingPart));
                        }
                        html.Append("</td>");
                    }
                    html.Append("</tr>");
                }
                html.Append("</table>");
            }
            // Add more element types as needed

            return html.ToString();
        }

        /// <summary>
        /// Converts a Run element to its HTML representation, preserving text styling.
        /// </summary>
        /// <param name="run">The Run element to convert.</param>
        /// <returns>A string containing the HTML representation of the Run element.</returns>
        private string ConvertRunToHtml(DocumentFormat.OpenXml.Wordprocessing.Run run)
        {
            StringBuilder html = new StringBuilder();
            var runProperties = run.RunProperties;

            if (runProperties != null)
            {
                html.Append("<span style='");
                if (runProperties.FontSize != null && int.TryParse(runProperties.FontSize.Val, out int fontSize))
                    html.Append($"font-size: {fontSize / 2}pt;");
                if (runProperties.Color != null)
                    html.Append($"color: #{runProperties.Color.Val};");
                if (runProperties.Bold != null)
                    html.Append("font-weight: bold;");
                if (runProperties.Italic != null)
                    html.Append("font-style: italic;");
                html.Append("'>");
            }
            else
            {
                html.Append("<span>");
            }

            html.Append(System.Net.WebUtility.HtmlEncode(run.InnerText));
            html.Append("</span>");

            return html.ToString();
        }
        #endregion

        #region IDisposable Implementation
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            // Dispose of managed resources
            _document = null;
            _documentContent = null;
        }
        #endregion
    }

    /// <summary>
    /// Represents the configuration for the help document.
    /// </summary>
    public class HelpDocumentInfo
    {
        /// <summary>
        /// Gets or sets the name of the SharePoint document library containing the help document.
        /// </summary>
        public string DocumentLibrary { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the name of the help document file.
        /// </summary>
        public string Document { get; set; } = string.Empty;
    }
}