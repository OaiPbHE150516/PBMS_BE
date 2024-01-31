﻿using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.Mail;

namespace pbms_be.Data.Auth;
[Table("account", Schema = "public")]

public class Account
{
    //     CREATE TABLE Account (
    //      AccountID serial PRIMARY KEY,
    //      UniqueID VARCHAR( 100 ) UNIQUE NOT NULL,
    //      ClientID VARCHAR( 100 ) UNIQUE NOT NULL,
    //      EmailAddress VARCHAR( 100 ) UNIQUE NOT NULL,
    //      AccountName VARCHAR( 100 ) UNIQUE NOT NULL,
    //      PictureURL VARCHAR( 500 ) UNIQUE NOT NULL,
    //      EnCodedJWT_IDtoken VARCHAR( 1000 ) UNIQUE NOT NULL
    //    );

    [Column("account_id")]
    public string AccountID { get; set; }

    [Column("client_id")]
    public string ClientID { get; set; }

    [Column("email_address")]
    public string EmailAddress { get; set; }

    [Column("account_name")]
    public string AccountName { get; set; }

    [Column("picture_url")]
    public string PictureURL { get; set; }

    //role id
    [Column("role_id")]
    public int RoleID { get; set; }

    // create_time
    [Column("create_time")]
    public DateTime CreateTime { get; set; }
}

