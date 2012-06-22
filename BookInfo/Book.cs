using System.Collections.Generic;

namespace BookInfo
{
    public class BookTag : Entity
    {
        public string Tag { get; set; }
        public string Count { get; set; }
    }

    public class BookRating : Entity
    {
        public string Average { get; set; }
        public string RatersNum { get; set; }
    }

    public class Review : Entity
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public string Summary { get; set; }
        public string Rating { get; set; }
        public string WebLink { get; set; }
        public string Published { get; set; }
    }

    public class BuyLink : Entity
    {
        public string Provider { get; set; }
        public string Price { get; set; }
    }

    public class Book : Entity
    {
        public Book() 
        {
            Tags = new List<BookTag>();
            Author = new List<string>();
            Translator = new List<string>();
            Rating = new BookRating();
            Title = "";
            Subtitle = "";
            AuthorIntro = "";
            Imageurl = "";
            Summary = "";
            Isbn = "";
            Pages = "";
            Price = "";
            Publisher = "";
            Pubdate = "";
            Binding = "";
            Weblink = "";
        }

        public string Title { get; set; }
        public string Subtitle { get; set; }
        public List<string> Author { get; set; }
        public List<string> Translator { get; set; }
        public string AuthorIntro { get; set; }
        public string Imageurl { get; set; }
        public string Summary { get; set; }
        public string Isbn { get; set; }
        public string Pages { get; set; }
        public string Price { get; set; }
        public string Publisher { get; set; }
        public string Pubdate { get; set; }
        public BookRating Rating { get; set; }
        public string Binding { get; set; }
        public List<BookTag> Tags { get; set; }
        public string Weblink { get; set; }
    }
}
