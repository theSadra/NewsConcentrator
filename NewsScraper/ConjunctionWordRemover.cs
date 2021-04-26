using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewsConcentratorSystem.NewsScraper
{
    public class ConjunctionWordRemover
    {
        private List<string> _conjuctionwordList;
        public ConjunctionWordRemover(List<string> ConjuctionwordList)
        {
            _conjuctionwordList = ConjuctionwordList;
        }


        public string RemoveConjunctions(string textstring)
        {
            var wordlist = textstring.Split(' ').ToList();
            wordlist.Except(_conjuctionwordList);


            string resultstring = null;

            foreach (var word in wordlist) 
                resultstring += wordlist + " ";

            return resultstring;
        }

    }
}
