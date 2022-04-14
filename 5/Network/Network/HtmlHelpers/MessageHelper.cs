using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

using Network.Models;

namespace Network.HtmlHelpers
{
    public static class MessageHelper
    {
        public static HtmlString CreateMessage(this IHtmlHelper html, Message msg, string status, bool replyButton = false)
        {
            var accordion = CreateAccordion();
            var accordionItem = CreateAccordionItem();
            var accordionHeader = CreateAccordionHeader(msg.Id, status);
            string headerButtonContent = $"Author: {msg.Author.Email} ({msg.Author.Name}), Created: {msg.Created}, Subject: {msg.Subject}";
            var accordionHeaderButton = CreateAccordionHeaderButton(msg.Id, status, headerButtonContent);
            var accordionCollapse = CreateAccordionCollapse(msg.Id, status);
            var accordionBody = CreateAccordionBody(html, msg, replyButton);

            accordion.InnerHtml.AppendHtml(accordionItem);
            accordionItem.InnerHtml.AppendHtml(accordionHeader);
            accordionItem.InnerHtml.AppendHtml(accordionCollapse);
            accordionHeader.InnerHtml.AppendHtml(accordionHeaderButton);
            accordionCollapse.InnerHtml.AppendHtml(accordionBody);

            var writer = new System.IO.StringWriter();
            accordion.WriteTo(writer, System.Text.Encodings.Web.HtmlEncoder.Default);
            return new HtmlString(writer.ToString());
        }

        private static TagBuilder CreateAccordion()
        {
            TagBuilder accordionDiv = new TagBuilder("div");
            accordionDiv.Attributes.Add("class", "accordion");
            return accordionDiv;
        }

        private static TagBuilder CreateAccordionItem()
        {
            TagBuilder accordionItemDiv = new TagBuilder("div");
            accordionItemDiv.Attributes.Add("class", "accordion-item");
            return accordionItemDiv;
        }

        private static TagBuilder CreateAccordionHeader(int id, string status)
        {
            TagBuilder h2 = new TagBuilder("h2");
            h2.Attributes.Add("class", "accordion-header");
            h2.Attributes.Add("id", $"accordion-header-{id}-{status}");
            return h2;
        }

        private static TagBuilder CreateAccordionHeaderButton(int id, string status, string content)
        {
            TagBuilder button = new TagBuilder("button");
            button.Attributes.Add("class", "accordion-button");
            button.Attributes.Add("type", "button");
            button.Attributes.Add("data-bs-toggle", "collapse");
            button.Attributes.Add("data-bs-target", $"#collapse-{id}-{status}");
            button.Attributes.Add("aria-expanded", "true");
            button.Attributes.Add("aria-controls", $"collapse-{id}-{status}");
            button.InnerHtml.Append(content);
            return button;
        }

        private static TagBuilder CreateAccordionCollapse(int id, string status)
        {
            TagBuilder collapse = new TagBuilder("div");
            collapse.Attributes.Add("class", "accordion-collapse collapse");
            collapse.Attributes.Add("id", $"collapse-{id}-{status}");
            collapse.Attributes.Add("aria-labelledby", $"collapse-{id}-{status}");
            return collapse;
        }

        private static TagBuilder CreateAccordionBody(this IHtmlHelper html, Message msg, bool replyButton)
        {
            TagBuilder body = new TagBuilder("div");
            body.Attributes.Add("class", "accordion-body");

            TagBuilder ul = CreateBodyUl(msg, replyButton);
            body.InnerHtml.AppendHtml(ul);
            body.InnerHtml.AppendLine();
            body.InnerHtml.Append(msg.Body);

            if (msg.ReplyTo != null)
            {
                body.InnerHtml.AppendLine();
                body.InnerHtml.AppendHtml(CreateMessage(html, msg.ReplyTo, $"reply-from-{msg.Id}"));
            }
            return body;
        }

        private static TagBuilder CreateBodyUl(Message msg, bool replyButton)
        {
            TagBuilder ul = new TagBuilder("ul");
            ul.Attributes.Add("class", "list-group list-group-flush");
            if (replyButton) ul.InnerHtml.AppendHtml(CreateReplyButton(msg.Id));
            string[] data =
            {
                $"Author: {msg.Author.Email} ({msg.Author.Name})",
                $"Adressees: " + String.Join("; ", Array.ConvertAll(msg.Addressees.ToArray(), a => $"{a.Email} ({a.Name})")),
                $"Subject: {msg.Subject}"
            };
            foreach (var d in data)
            {
                TagBuilder li = new TagBuilder("li");
                li.Attributes.Add("class", "list-group-item");
                li.InnerHtml.Append(d);
                ul.InnerHtml.AppendHtml(li);
            }
            return ul;
        }

        private static TagBuilder CreateReplyButton(int id)
        {
            TagBuilder button = new TagBuilder("a");
            button.Attributes.Add("class", "btn btn-primary");
            button.Attributes.Add("href", $"/Message/Reply/?id={id}");
            button.InnerHtml.Append("Reply");
            return button;
        }
    }
}

/*

<div class="accordion">
    <div class="accordion-item">
        <h2 class="accordion-header" id="panelsStayOpen-heading-@message.Id-sent">
            <button class="accordion-button" type="button" data-bs-toggle="collapse" data-bs-target="#panelsStayOpen-collapse-@message.Id-sent" aria-expanded="true" aria-controls="panelsStayOpen-collapse-@message.Id-sent">
                Author: @message.Author.Email (@message.Author.Name), Created: @message.Created, Subject: @message.Subject
            </button>
        </h2>
        <div id="panelsStayOpen-collapse-@message.Id-sent" class="accordion-collapse collapse" aria-labelledby="panelsStayOpen-heading-@message.Id-sent">
            <div class="accordion-body">
                <ul class="list-group list-group-flush">
                    <li class="list-group-item">Author: @message.Author.Email (@message.Author.Name)</li>
                    <li class="list-group-item">Adressees: @String.Join(";", Array.ConvertAll(message.Addressees.ToArray(), a => $"{a.Email} ({a.Name})"))</li>
                </ul>
                <br />
                @message.Body
            </div>
        </div>
    </div>
</div>
 
 */