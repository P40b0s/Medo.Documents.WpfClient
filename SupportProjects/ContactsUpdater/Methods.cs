using ContactsUpdater.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContactsUpdater
{
    class Methods : MedoContactsModel
    {

        public Methods()
        {
          
        }
        public void OpenNewContactDialog()
        {
            CreateNewContactWindowIsOpen = true;
        }

    }
}
