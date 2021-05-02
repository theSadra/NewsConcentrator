using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NewsConcentratorSystem.Models;
using TLSharp;

namespace NewsConcentratorSystem.NewsScraper
{
    public class RepetitiousNewsTextDetector
    {
        private IQueryable<News> _publishedNewses;
        private double _similarityPercentage;
        private News currentNews;

        public RepetitiousNewsTextDetector(IQueryable<News> publisedneNewses, int similarityPercentage)
        {
            _publishedNewses = publisedneNewses;
            _similarityPercentage = similarityPercentage;
        }


        public bool IsNewstextRepetitous(News news)
        {
            currentNews = news;

            foreach (var pnews in _publishedNewses)
            {
                var pnewssplitedwords = pnews.TextMessage.Split(' ').AsQueryable();
                var newssplitedwords = news.TextMessage.Split(' ').AsQueryable();
                int commonwords = newssplitedwords.Intersect(pnewssplitedwords).Count();
                var lowestextnews = pnewssplitedwords.Count() > newssplitedwords.Count()
                    ? newssplitedwords.Count()
                    : pnewssplitedwords.Count();
                //if the common words count on News, be same or higher than _similarityPercentage => is same news
                double samepercentage = ((double)commonwords / (double)lowestextnews)*100;
                Console.WriteLine($"______--------{samepercentage}--------______");
                if (samepercentage >= _similarityPercentage)
                {
                    //Same news :)
                    return true;
                }

            }

            return false;

        }




    }
}
