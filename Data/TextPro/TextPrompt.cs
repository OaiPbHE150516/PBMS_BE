using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace pbms_be.Data.TextPro;
[Table("text_prompt", Schema = "public")]

public class TextPrompt
{
    /*
        CREATE TABLE text_prompt (
            text_prompt_id SERIAL PRIMARY KEY,
            text_action VARCHAR(255) NOT NULL,
            text_prompt TEXT NOT NULL
        ); 
     */

    [Column("text_prompt_id")]
    [Key]
    public int TextPromptID { get; set; }

    [Column("text_action")]
    public string TextAction { get; set; } = String.Empty;

    [Column("text_prompt")]
    public string Text_Prompt { get; set; } = String.Empty;
}
