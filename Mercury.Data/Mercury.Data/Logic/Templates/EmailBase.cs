using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mercury.Data.Util;
using System.IO;
using Mercury.Model;

namespace Mercury.Data.Logic
{
    public abstract class EmailBase : IEmail 
    {
        private string subject;
        private string template;
        private string resourcePath;
        private EmailType emailType;
        private ITemplateContext context;

        public EmailBase(string subject, string resourcePath) : this(subject, resourcePath, EmailType.General) { }

        public EmailBase(string subject, string resourcePath, EmailType emailType)
        {
            this.subject = subject;
            this.resourcePath = resourcePath;
            this.emailType = EmailType.General;

            //Get template using resource path
            LoadTemplate();
        }

        public void LoadTemplate()
        {
            if (String.IsNullOrEmpty(resourcePath)) //Don't try to load if resource path isn't set
                return;

            using(var stream = EmbeddedResourceHelper.GetEmbeddedFile(typeof(IEmail), resourcePath)){
                using (var reader = new StreamReader(stream))
                {
                    this.Template = reader.ReadToEnd();
                }
            }            
        }

        public virtual ITemplateContext CreateContextObject()
        {
            this.Context = new DefaultTemplateContext();
            return this.Context;
        }

        public ITemplateContext Context
        {
            get { return context; }
            set { context = value; }
        }
        
        #region IEmail Members

        public string Subject
        {
            get
            {
                return subject;
            }
            set
            {
                subject = value;
            }
        }

        public string Template
        {
            get
            {
                return template;
            }
            set
            {
                template = value;
            }
        }

        public string ResourcePath
        {
            get
            {
                return resourcePath;
            }
            set
            {
                resourcePath = value;
            }
        }

        public EmailType EmailType
        {
            get { return emailType; }
            set { emailType = value; }
        }

        #endregion
    }
}
