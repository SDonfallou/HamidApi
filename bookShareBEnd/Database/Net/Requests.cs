﻿using System.ComponentModel.DataAnnotations;

namespace bookShareBEnd.Database.Net;

public class LoginRequest
{
   [Required]
   public string UserName { get; set; }


}
