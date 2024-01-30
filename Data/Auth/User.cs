using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

namespace pbms_be.Data.Auth;
[Table("pbms_user", Schema = "public")]

public class User
{
    // CREATE TABLE pbms_user(
    //    user_id serial PRIMARY KEY,
    // account_id INT NOT NULL,
    // role_id INT NOT NULL,
    // create_time TIMESTAMP NOT NULL,
    //    FOREIGN KEY (account_id) REFERENCES account (account_id),
    //    FOREIGN KEY (role_id) REFERENCES role (role_id)
    //);

    [Column("user_id")]
    public int UserID { get; set; }

    [Column("account_id")]
    public int AccountID { get; set; }
    public virtual Account Account { get; set; }

    [Column("role_id")]
    public int RoleID { get; set; }
    public virtual Role Role { get; set; }

    [Column("create_time")]
    public DateTime CreateTime { get; set; }
}

