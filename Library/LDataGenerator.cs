using Microsoft.Identity.Client;
using pbms_be.Configurations;
using pbms_be.Data.Custom;
using System.Linq;
using System.Text;
using System.Transactions;
using pbms_be.Data.Trans;

namespace pbms_be.Library
{
    public class LDataGenerator
    {
        private static long GenerateRandomNumberInRange(long min, long max)
        {
            var random = new Random();
            return random.Next((int)min, (int)max);
        }

        // generate random money amount in range
        private static long GenerateRandomMoneyAmountInRange(long min, long max, bool isRound)
        {
            var random = new Random();
            // make last 3 digits to be 0
            var result = random.Next((int)min, (int)max);
            if (isRound) return result - result % 1000;
            return result;
        }

        // random item in a array of string
        private static string GenerateRandomStringFromArray(string[] array)
        {
            var random = new Random();
            return array[random.Next(0, array.Length)];
        }

        private static int GenerateRandomIntFromList(List<int> list)
        {
            var random = new Random();
            return list[random.Next(0, list.Count)];
        }

        private static string GenerateRandomString(int maxlength)
        {
            // generate random string with random length
            var random = new Random();
            var length = random.Next(1, maxlength);
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var result = new string(Enumerable.Repeat(chars, length)
                                            .Select(s => s[random.Next(s.Length)]).ToArray());
            return result;
        }

        // generate random string with random length, input string
        private static string GenerateRandomString(int minLength, int maxLength, string input)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringToUse = string.IsNullOrEmpty(input) ? chars : input + chars;
            var random = new Random();
            var length = random.Next(minLength, maxLength);
            var result = new string(Enumerable.Repeat(stringToUse, length)
                                            .Select(s => s[random.Next(s.Length)]).ToArray());
            return result;
        }

        // generate random paragraph with random length
        private static string GenerateRandomParagraph(int minLength, int maxLength)
        {
            var random = new Random();
            var length = random.Next(minLength, maxLength);
            var result = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                result.Append(GenerateRandomString(100));
                result.Append(" ");
            }
            return result.ToString();
        }        
        private static string GenerateRandomParagraphLorem(int length)
        {
            var lorem = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. " +
                "Pellentesque mollis neque risus, nec fermentum nisi egestas a. " +
                "Curabitur consectetur erat quis massa fermentum pretium. " +
                "Quisque in augue eu augue consectetur pellentesque. " +
                "Phasellus interdum elit eleifend massa tempor porttitor. " +
                "Morbi varius volutpat tortor, ac laoreet justo. " +
                "Donec id leo non lectus tincidunt lacinia. " +
                "Phasellus non purus non nisl auctor molestie." +
                "In ut nisi in ligula dapibus tristique. " +
                "Morbi eget enim ornare, ultricies ipsum non, semper neque. " +
                "Suspendisse nec malesuada sem. " +
                "Nam vel est rhoncus, tincidunt massa et, sodales lorem. " +
                "Nunc gravida ligula vel justo mollis tincidunt posuere sed libero. " +
                "Aenean quam arcu, elementum non viverra vitae, iaculis vitae orci. " +
                "Nunc non turpis vitae tellus mollis suscipit vitae eu nisl. " +
                "Nam commodo et odio et vehicula. " +
                "Pellentesque imperdiet porta lobortis. " +
                "Ut iaculis commodo quam ut imperdiet. " +
                "Morbi accumsan vitae ipsum a sollicitudin. " +
                "Quisque mollis magna est, ut mollis mauris viverra faucibus. " +
                "Praesent vitae dolor leo. Donec mollis lectus lectus, " +
                "et molestie quam semper eu. Ut ornare tortor non posuere vestibulum. " +
                "Morbi at cursus dolor, vitae aliquet dolor. " +
                "Mauris sapien turpis, efficitur in tellus et, ultricies gravida tellus. " +
                "Suspendisse vulputate, ex feugiat porttitor fringilla, mauris felis placerat erat, " +
                "a mollis sem elit bibendum lacus. Phasellus id congue orci. Ut ut orci ac massa " +
                "sollicitudin lobortis ac eget lacus. Suspendisse nec nulla hendrerit, suscipit lacus vel, " +
                "blandit nunc. Fusce placerat lacus dolor, egestas tincidunt justo pretium sit amet. " +
                "Sed bibendum ante elit, luctus cursus leo pharetra eget. Morbi mollis a nibh vitae consequat. " +
                "Mauris nec luctus enim, vel commodo mauris. Maecenas dictum neque turpis, ac volutpat erat congue vel. " +
                "Interdum et malesuada fames ac ante ipsum primis in faucibus. Proin quis vestibulum orci, " +
                "vitae blandit quam. Suspendisse sit amet ipsum ligula. Maecenas sed pharetra ipsum. " +
                "Vestibulum maximus consectetur turpis ultricies dignissim. " +
                "Phasellus dictum fringilla sapien, nec sagittis eros convallis varius. " +
                "Cras porta est non egestas blandit. Etiam quis bibendum ex. " +
                "Morbi commodo leo eget ante porttitor sodales. Sed porttitor placerat mi, laoreet blandit arcu efficitur vitae. " +
                "Mauris in massa arcu. Nunc ut pulvinar nulla, at tristique nibh. Donec vestibulum iaculis tortor sed fringilla. " +
                "Etiam rhoncus massa ac vehicula finibus. Mauris ut elit pharetra, hendrerit turpis vitae, semper dolor. " +
                "Vestibulum condimentum nibh quis lacinia malesuada. Sed risus leo, tempor quis risus sit amet, placerat luctus urna." +
                "Aliquam suscipit interdum mauris at scelerisque. Duis pretium feugiat tortor in accumsan. " +
                "Mauris malesuada ipsum mi, quis blandit orci efficitur eu. Vestibulum in mollis erat. " +
                "Donec eleifend convallis finibus. Praesent nibh odio, rutrum vitae dolor non, tempor ullamcorper nulla. " +
                "Suspendisse finibus dictum risus et posuere. Praesent ultricies molestie ligula. " +
                "Nullam posuere, metus vel mattis pulvinar, libero felis semper tortor, ut placerat urna ligula sed est. " +
                "Cras scelerisque nunc quis magna malesuada, ac vehicula mi hendrerit. " +
                "Proin rutrum accumsan risus, at fermentum est luctus ac. Morbi sit amet massa placerat, tempus ex non, " +
                "vestibulum lacus. Proin fringilla eros eget eros rhoncus sagittis.";
            // split the string into words by space, ', ' and '. '
            var words = lorem.Split(new string[] { " ", ", ", ". " }, StringSplitOptions.RemoveEmptyEntries);
            var random = new Random();
            var result = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                result.Append(words[random.Next(0, words.Length)].ToLower());
                var randomChar = random.Next(0, 3);
                if (randomChar == 0)
                {
                    result.Append(" ");
                }
                else if (randomChar == 1)
                {
                    result.Append(", ");
                }
                else
                {
                    result.Append(". ");
                }
            }
            // remove double space, comma, dot
            var resultStr = result.ToString();
            resultStr = resultStr.Replace("  ", " ");
            resultStr = resultStr.Replace(", ,", ", ");
            resultStr = resultStr.Replace(",,", ", ");
            resultStr = resultStr.Replace(". .", ". ");
            resultStr = resultStr.Replace("..", ". ");
            return resultStr;
        }

        private static string GenerateRandomDateTimeInRangeStr(DateTime min, DateTime max)
        {
            var random = new Random();
            var range = max - min;
            var randomTimeSpan = new TimeSpan((long)(random.NextDouble() * range.Ticks));
            var randomDate = min + randomTimeSpan;
            return randomDate.ToString("yyyy-MM-dd HH:mm:ss");
        }

        // GenerateRandomDateTimeInRange
        private static DateTime GenerateRandomDateTimeInRange(DateTime min, DateTime max)
        {
            var random = new Random();
            var range = max - min;
            var randomTimeSpan = new TimeSpan((long)(random.NextDouble() * range.Ticks));
            var randomDate = min + randomTimeSpan;
            // convert to utc time
            randomDate = LConvertVariable.ConvertLocalToUtcTime(randomDate);
            return randomDate;
        }

        public static object GenerateDataBySqlQuery()
        {
            // create a txt file with random data and save it to new file in folder 'File' in local storage
            //var randomData = LConvertVariable.GenerateRandomString(1000, "nothing");
            var fileName = "randomData.txt";
            var folder = "File";
            var path = Path.Combine(Directory.GetCurrentDirectory(), folder, fileName);
            //System.IO.File.WriteAllText(path, randomData);
            var defaultImageURL = "https://storage.googleapis.com/pbms-user/collabfund/default-image.jpg";
            var accountID = "117911566377016615313";
            var walletIDArray = new string[] { "2", "3", "4", "5" };
            var categoryIDArray = new string[] { "338", "339", "341", "342", "343", "344", "345", "346", "347", "348", "349", "350", "351", "352", "353" };

            var randomData = new StringBuilder();
            randomData.Append("INSERT INTO transaction (account_id, wallet_id, category_id, total_amount, note, transaction_date, from_person, to_person, image_url, as_id)\r\nVALUES \r\n");
            var minDateTime = new DateTime(2020, 1, 1);
            var maxDateTime = new DateTime(2024, 12, 30);
            // loop 1000 times to create 1000 random data
            var length = 1000;
            for (int i = 0; i < length; i++)
            {
                var stringBuilder = new StringBuilder();
                stringBuilder.Append("(");
                stringBuilder.Append("'" + accountID + "', ");
                stringBuilder.Append(GenerateRandomStringFromArray(walletIDArray) + ", ");
                stringBuilder.Append(GenerateRandomStringFromArray(categoryIDArray) + ", ");
                stringBuilder.Append(GenerateRandomMoneyAmountInRange(1000, 10000000, false) + ", ");
                stringBuilder.Append("'" + GenerateRandomString(100) + "', ");
                stringBuilder.Append("'" + GenerateRandomDateTimeInRangeStr(minDateTime, maxDateTime) + "', ");
                // generate random string with random length
                stringBuilder.Append("'" + GenerateRandomString(20) + "', ");
                stringBuilder.Append("'" + GenerateRandomString(25) + "', ");
                // default image url
                stringBuilder.Append("'" + defaultImageURL + "', ");
                stringBuilder.Append(1);
                // if it's the last item, don't add comma
                if (i == length - 1)
                {
                    stringBuilder.Append(");");
                }
                else
                {
                    stringBuilder.Append("), ");
                    stringBuilder.Append("\r\n");
                }
                randomData.Append(stringBuilder);
            }
            randomData.Append("\r\n");
            System.IO.File.WriteAllText(path, randomData.ToString());
            return "name";
        }

        internal static object GenerateRandomTransactionsEF(GenerateRandomTransactions data, Data.PbmsDbContext _context)
        {
            try
            {
                var before = _context.Transaction
                .Where(t => t.AccountID == data.AccountID && t.ActiveStateID == ActiveStateConst.ACTIVE)
                .Count();

                var transactions = new List<Data.Trans.Transaction>();
                var random = new Random();
                var minDateTime = new DateTime(data.minYear, data.minMonth, data.minDay);
                var maxDateTime = new DateTime(data.maxYear, data.maxMonth, data.maxDay);

                var wallets = new List<int>();
                wallets = _context.Wallet
                    .Where(w => w.AccountID == data.AccountID && w.ActiveStateID == ActiveStateConst.ACTIVE)
                    .Select(w => w.WalletID).ToList();
                if (wallets.Count == 0) throw new Exception("No wallet found for account " + data.AccountID);

                var categories = new List<int>();
                categories = _context.Category
                    .Where(c => c.AccountID == data.AccountID && c.ActiveStateID == ActiveStateConst.ACTIVE)
                    .Select(c => c.CategoryID).ToList();
                if (categories.Count == 0) throw new Exception("No category found for account " + data.AccountID);

                for (int i = 0; i < data.numberOfTransactions; i++)
                {
                    var transaction = new Data.Trans.Transaction
                    {
                        AccountID = data.AccountID
                    };
                    transaction.AccountID = data.AccountID;
                    transaction.WalletID = wallets[random.Next(0, wallets.Count)];
                    transaction.CategoryID = categories[random.Next(0, categories.Count)];
                    transaction.TotalAmount = GenerateRandomMoneyAmountInRange(data.minAmount, data.maxAmount, data.isRoundAmount);
                    transaction.Note = GenerateRandomParagraphLorem(20);
                    transaction.TransactionDate = GenerateRandomDateTimeInRange(minDateTime, maxDateTime);
                    transaction.FromPerson = GenerateRandomParagraphLorem(2);
                    transaction.ToPerson = GenerateRandomParagraphLorem(3);
                    transaction.ImageURL = "https://storage.googleapis.com/pbms-user/collabfund/default-image.jpg";
                    transaction.ActiveStateID = ActiveStateConst.ACTIVE;
                    transactions.Add(transaction);
                }
                _context.Transaction.AddRange(transactions);
                _context.SaveChanges();

                var after = _context.Transaction
                    .Where(t => t.AccountID == data.AccountID && t.ActiveStateID == ActiveStateConst.ACTIVE)
                    .Count();
                var description = "Generate " + (after - before).ToString() + " random transactions for account " + data.AccountID;
                var compare = "before: " + before + ", after: " + after;
                // return the number of transactions before and after
                return new { description, compare };
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
