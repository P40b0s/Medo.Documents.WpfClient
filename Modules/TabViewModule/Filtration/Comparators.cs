using Medo.Core.Models;
using System.Collections.Generic;
using System.ComponentModel;

namespace Medo.Modules.TabViewModule.Filtration
{
    class Comparators
    {
        public class OrganComparer : IComparer<Document>
        {
            public int Compare(Document x, Document y)
            {
                return string.Compare(x.OrganName, y.OrganName);
            }
        }

        public class ActTypeComparer : IComparer<Document>
        {
            public int Compare(Document x, Document y)
            {
                return string.Compare(x.ActType, y.ActType);
            }
        }

        public class EoNumberComparer : IComparer<Document>
        {
            public int Compare(Document x, Document y)
            {
                return string.Compare(x.EoNumber, y.EoNumber);
            }
        }

        public class DeliveryTimeComparer : IComparer<Document>
        {
            ListSortDirection Direction { get; set; }
            public DeliveryTimeComparer(ListSortDirection Direction)
            {
                this.Direction = Direction;
            }
            public int Compare(Document x, Document y)
            {
                int i = 0;
                switch (Direction)
                {
                    default:
                        i = 0;
                        break;
                    case ListSortDirection.Descending:
                        {
                            if (x.DeliveryTime < y.DeliveryTime) i = 1;
                            if (x.DeliveryTime > y.DeliveryTime) i = -1;
                            break;
                        }
                    case ListSortDirection.Ascending:
                        {
                            if (x.DeliveryTime < y.DeliveryTime) i = -1;
                            else if (x.DeliveryTime > y.DeliveryTime) i = 1;
                            break;
                        }
                }
                return i;
            }
        }

        public class SignDateComparer : IComparer<Document>
        {
            ListSortDirection Direction { get; set; }
            public SignDateComparer(ListSortDirection Direction)
            {
                this.Direction = Direction;
            }
            public int Compare(Document x, Document y)
            {
                int i = 0;
                switch (Direction)
                {
                    default:
                        i = 0;
                        break;
                    case ListSortDirection.Descending:
                        {
                            if (x.SignDate < y.SignDate) i = 1;
                            if (x.SignDate > y.SignDate) i = -1;
                            break;
                        }
                    case ListSortDirection.Ascending:
                        {
                            if (x.SignDate < y.SignDate) i = -1;
                            else if (x.SignDate > y.SignDate) i = 1;
                            break;
                        }
                }
                return i;
            }
        }

        public class PublicationDateComparer : IComparer<Document>
        {
            ListSortDirection Direction { get; set; }
            public PublicationDateComparer(ListSortDirection Direction)
            {
                this.Direction = Direction;
            }
            public int Compare(Document x, Document y)
            {
                int i = 0;
                switch (Direction)
                {
                    default:
                        i = 0;
                        break;
                    case ListSortDirection.Descending:
                        {
                            if (x.PublDatePortal < y.PublDatePortal) i = 1;
                            if (x.PublDatePortal > y.PublDatePortal) i = -1;
                            break;
                        }
                    case ListSortDirection.Ascending:
                        {
                            if (x.PublDatePortal < y.PublDatePortal) i = -1;
                            else if (x.PublDatePortal > y.PublDatePortal) i = 1;
                            break;
                        }
                }
                return i;
            }
        }



        public class ActNumberComparer : IComparer<Document>
        {
            ListSortDirection Direction { get; set; }
            public ActNumberComparer(ListSortDirection Direction)
            {
                this.Direction = Direction;
            }

            public int Compare(Document x, Document y)
            {
                int i = 0;
                switch (Direction)
                {
                    default:
                        return 0;
                    case ListSortDirection.Descending:
                        {
                            if ((x.ClearNumber < y.ClearNumber)) i = 1;
                            if ((x.ClearNumber > y.ClearNumber)) i = -1;
                            break;
                        }
                    case ListSortDirection.Ascending:
                        {
                            if ((x.ClearNumber < y.ClearNumber)) i = -1;
                            if ((x.ClearNumber > y.ClearNumber)) i = 1;
                            break;
                        }
                }
                return i;
            }
        }
    }
}
