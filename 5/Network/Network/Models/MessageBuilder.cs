namespace Network.Models
{
    public class MessageBuilder
    {
        private Message message;

        public MessageBuilder(NetworkContext db)
        {
            message = new Message();
            message.Addressees = new List<User>();
        }

        public MessageBuilder SetSubject(string subject)
        {
            message.Subject = subject;
            return this;
        }

        public MessageBuilder SetBody(string body)
        {
            message.Body = body;
            return this;
        }

        public MessageBuilder SetAuthor(User? author)
        {
            if (author == null) return this;
            message.Author = author;
            return this;
        }

        public MessageBuilder AddAddressee(params User?[] addressees)
        {
            if (addressees == null) return this;
            foreach(var addressee in addressees)
            {
                if (addressee != null && !message.Addressees.Contains(addressee))
                    message.Addressees.Add(addressee);
            }
            return this;
        }

        public MessageBuilder RepliesTo(Message? msg)
        {
            message.ReplyTo = msg;
            return this;
        }

        public Message? ToMessage()
        {
            if (message.Author == null) return null;
            if (message.Addressees == null || message.Addressees.Count == 0) return null;
            message.Created = DateTime.Now;
            return message;
        }
    }
}
