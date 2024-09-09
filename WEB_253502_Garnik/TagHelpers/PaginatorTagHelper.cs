using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Routing;
using Microsoft.CodeAnalysis.Text;
using System.Text;
using System.Threading.Tasks;

public class PaginatorTagHelper : TagHelper {
    private readonly LinkGenerator _linkGenerator;

    // Внедрение LinkGenerator через конструктор
    public PaginatorTagHelper(LinkGenerator linkGenerator) {
        _linkGenerator = linkGenerator;
    }

    // Параметры пагинатора
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public string CurrentCategory { get; set; }
    public int Page1 { get; set; }
    public int Page2 { get; set; }
    public int Page3 { get; set; }
    public bool Admin { get; set; } = false;
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output) {
        output.TagName = "nav"; // This will output a <nav> tag
        output.Attributes.SetAttribute("aria-label", "Page navigation");

        var sb = new StringBuilder();
        sb.Append(@"<ul class=""pagination"">");

        if (!Admin) {
            // Генерация ссылок с помощью LinkGenerator
            var prevLink = _linkGenerator.GetPathByAction(
            action: "Catalog",
            controller: "Product",
            values: new { pageNo = CurrentPage - 1, category = CurrentCategory });

            var nextLink = _linkGenerator.GetPathByAction(
                action: "Catalog",
                controller: "Product",
                values: new { pageNo = CurrentPage + 1, category = CurrentCategory });

            // Previous button
            sb.Append($@"<li class=""page-item {(CurrentPage == 1 ? "disabled" : "")}"">
                         <a class=""page-link"" href=""{prevLink}"">Previous</a>
                     </li>");

            // Page buttons
            if (Page1 != 0) {
                var page1Link = _linkGenerator.GetPathByAction(
                    action: "Catalog",
                    controller: "Product",
                    values: new { pageNo = Page1, category = CurrentCategory });

                sb.Append($@"<li class=""page-item {(Page1 == CurrentPage ? "active" : "")}"">
                             <a class=""page-link"" href=""{page1Link}"">{Page1}</a>
                         </li>");
            }

            if (Page2 != 0) {
                var page2Link = _linkGenerator.GetPathByAction(
                    action: "Catalog",
                    controller: "Product",
                    values: new { pageNo = Page2, category = CurrentCategory });

                sb.Append($@"<li class=""page-item {(Page2 == CurrentPage ? "active" : "")}"">
                             <a class=""page-link"" href=""{page2Link}"">{Page2}</a>
                         </li>");
            }

            if (Page3 != 0) {
                var page3Link = _linkGenerator.GetPathByAction(
                    action: "Catalog",
                    controller: "Product",
                    values: new { pageNo = Page3, category = CurrentCategory });

                sb.Append($@"<li class=""page-item {(Page3 == CurrentPage ? "active" : "")}"">
                             <a class=""page-link"" href=""{page3Link}"">{Page3}</a>
                         </li>");
            }

            // Next button
            sb.Append($@"<li class=""page-item {(CurrentPage >= TotalPages ? "disabled" : "")}"">
                         <a class=""page-link"" href=""{nextLink}"">Next</a>
                     </li>");
        } 
        else {
            var prevLink = _linkGenerator.GetPathByPage(
                page: "/Areas/Admin/Pages/Index",
                values: new { pageNo = CurrentPage - 1, category = CurrentCategory });

            var nextLink = _linkGenerator.GetPathByPage(
                page: "/Areas/Admin/Pages/Index",
                values: new { pageNo = CurrentPage + 1, category = CurrentCategory });

            // Previous button
            sb.Append($@"<li class=""page-item {(CurrentPage == 1 ? "disabled" : "")}"">
                         <a class=""page-link"" href=""{prevLink}"">Previous</a>
                     </li>");

            // Page buttons
            if (Page1 != 0) {
                var page1Link = _linkGenerator.GetPathByPage(
                page: "/Areas/Admin/Pages/Index",
                    values: new { pageNo = Page1, category = CurrentCategory });

                sb.Append($@"<li class=""page-item {(Page1 == CurrentPage ? "active" : "")}"">
                             <a class=""page-link"" href=""{page1Link}"">{Page1}</a>
                         </li>");
            }

            if (Page2 != 0) {
                var page2Link = _linkGenerator.GetPathByPage(
                page: "/Areas/Admin/Pages/Index",
                    values: new { pageNo = Page2, category = CurrentCategory });

                sb.Append($@"<li class=""page-item {(Page2 == CurrentPage ? "active" : "")}"">
                             <a class=""page-link"" href=""{page2Link}"">{Page2}</a>
                         </li>");
            }

            if (Page3 != 0) {
                var page3Link = _linkGenerator.GetPathByPage(
                page: "/Areas/Admin/Pages/Index",
                    values: new { pageNo = Page3, category = CurrentCategory });

                sb.Append($@"<li class=""page-item {(Page3 == CurrentPage ? "active" : "")}"">
                             <a class=""page-link"" href=""{page3Link}"">{Page3}</a>
                         </li>");
            }

            // Next button
            sb.Append($@"<li class=""page-item {(CurrentPage >= TotalPages ? "disabled" : "")}"">
                         <a class=""page-link"" href=""{nextLink}"">Next</a>
                     </li>");
        }
        sb.Append("</ul>");

        // Вывод готового HTML
        output.Content.SetHtmlContent(sb.ToString());
    }
}
