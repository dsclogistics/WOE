using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebOrderEntry
{
    public class CustomError : System.Web.UI.IValidator
    {
        private string errorMessage;

        public CustomError(string errorMessage)
        {
            this.errorMessage = errorMessage;
        }

        public string ErrorMessage
        {
            get
            {
                return errorMessage;
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        public bool IsValid
        {
            get
            {
                return false;
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        public void Validate()
        { }
    }
}
