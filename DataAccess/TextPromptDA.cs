using pbms_be.Data;

namespace pbms_be.DataAccess
{
    public class TextPromptDA
    {
        private readonly PbmsDbContext _context;

        public TextPromptDA(PbmsDbContext context)
        {
            _context = context;
        }

        public string? GetTextPrompt(string textAction)
        {
            try
            {
                var textPrompt = _context.TextPrompt.FirstOrDefault(x => x.TextAction == textAction);
                if (textPrompt == null) return null;
                return textPrompt.Text_Prompt;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
