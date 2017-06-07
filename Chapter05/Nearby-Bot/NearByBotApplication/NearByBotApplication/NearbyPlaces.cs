using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NearByBotApplication
{
    public class SampleDataItem
    {
        public SampleDataItem(String uniqueId, String title, String subtitle, String imagePath, String description, String content)
        {
            this.UniqueId = uniqueId;
            this.Title = title;
            this.Subtitle = subtitle;
            this.Description = description;
            this.ImagePath = imagePath;
            this.Content = content;
        }

        public string UniqueId { get; private set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string Description { get; private set; }
        public string ImagePath { get; private set; }
        public string Content { get; private set; }

        public override string ToString()
        {
            return this.Title;
        }
    }

    /// <summary>
    /// Generic group data model.
    /// </summary>
    public class SampleDataGroup
    {
        public SampleDataGroup(String uniqueId, String title, String subtitle, String imagePath, String description)
        {
            this.UniqueId = uniqueId;
            this.Title = title;
            this.Subtitle = subtitle;
            this.Description = description;
            this.ImagePath = imagePath;
            this.Items = new ObservableCollection<SampleDataItem>();
        }

        public string UniqueId { get; private set; }
        public string Title { get; private set; }
        public string Subtitle { get; private set; }
        public string Description { get; private set; }
        public string ImagePath { get; private set; }
        public ObservableCollection<SampleDataItem> Items { get; private set; }

        public override string ToString()
        {
            return this.Title;
        }
    }

    /// <summary>
    /// Creates a collection of groups and items with content read from a static json file.
    /// 
    /// SampleDataSource initializes with data read from a static json file included in the 
    /// project.  This provides sample data at both design-time and run-time.
    /// </summary>
    public sealed class SampleDataSource
    {
        public static ObservableCollection<SampleDataItem> AllItems = new ObservableCollection<SampleDataItem>();
        private static SampleDataSource _sampleDataSource = new SampleDataSource();

        private ObservableCollection<SampleDataGroup> _groups = new ObservableCollection<SampleDataGroup>();
        public ObservableCollection<SampleDataGroup> Groups
        {
            get { return this._groups; }
        }

        public static async Task<IEnumerable<SampleDataGroup>> GetGroupsAsync()
        {
            await _sampleDataSource.GetSampleDataAsync();

            return _sampleDataSource.Groups;
        }

        public static async Task<SampleDataGroup> GetGroupAsync(string uniqueId)
        {
            await _sampleDataSource.GetSampleDataAsync();
            // Simple linear search is acceptable for small data sets
            var matches = _sampleDataSource.Groups.Where((group) => group.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        public static async Task<SampleDataItem> GetItemAsync(string uniqueId)
        {
            await _sampleDataSource.GetSampleDataAsync();
            // Simple linear search is acceptable for small data sets
            var matches = _sampleDataSource.Groups.SelectMany(group => group.Items).Where((item) => item.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }


        public async static Task<IEnumerable<SampleDataItem>> GetMatchingItems(string query)
        {
            if (AllItems.Count == 0)
            {

                var allGroups = await SampleDataSource.GetGroupsAsync();
                foreach (var group in allGroups)
                {
                    foreach (var item in group.Items)
                        AllItems.Add(item);
                }
            }



            return AllItems
                .Where(c => c.UniqueId.IndexOf(query, StringComparison.CurrentCultureIgnoreCase) > -1
                            )
                .OrderByDescending(c => c.UniqueId.StartsWith(query, StringComparison.CurrentCultureIgnoreCase));
        }

        private async Task GetSampleDataAsync()
        {
            if (this._groups.Count != 0)
                return;

            var group1 = new SampleDataGroup("Group-A",
                    "A",
                    "",
                    "",
                    "");

            group1.Items.Add(new SampleDataItem("ATM",
                    "ATM",
                    "atm",
                    "Assets/ATM.png",
                    "",
                    ""
                    ));
            group1.Items.Add(new SampleDataItem("Airport",
                    "Air Port",
                    "airport",
                    "Assets/Flight2.png",
                    "",
                    ""
                    ));
            group1.Items.Add(new SampleDataItem("Accountant",
                   "Accountant",
                   "finance",
                   "Assets/Accounts-Book.png",
                   "",
                   ""
                   ));


            this._groups.Add(group1);

            var group2 = new SampleDataGroup("Group-B",
                    "B",
                    "",
                    "",

                    "");

            group2.Items.Add(new SampleDataItem("Bank",
                    "Bank",
                    "bank",
                    "Assets/Bank.png",
                    "",
                    ""
                    ));
            group2.Items.Add(new SampleDataItem("Bar",
                   "Bar",
                   "bar",
                   "Assets/Bar.png",
                   "",
                   ""
                   ));
            group2.Items.Add(new SampleDataItem("Book Store",
                   "Book Store",
                   "book_store",
                   "Assets/book store.png",
                   "",
                   ""
                   ));
            group2.Items.Add(new SampleDataItem("Bus Station",
                   "Bus Station",
                   "bus_station",
                   "Assets/Bus Station.png",
                   "",
                   ""
                   ));


            this._groups.Add(group2);


            var groupC = new SampleDataGroup("Group-C",
                "C",
                "",
                "",

                "");

            groupC.Items.Add(new SampleDataItem("Car Rental",
                    "Car Rental",
                    "car_rental",
                    "Assets/car rental.png",
                    "",
                    ""
                    ));
            groupC.Items.Add(new SampleDataItem("Car Wash",
                    "Car Wash",
                    "car_wash",
                    "Assets/car wash.png",
                    "",
                    " "
                    ));
            groupC.Items.Add(new SampleDataItem("Church",
                    "Church",
                    "church",
                    "Assets/Church.png",
                    "",
                    " "
                    ));
            groupC.Items.Add(new SampleDataItem("Cinema",
                    "Cinema",
                    "movie_theater",
                    "Assets/cinema.png",
                    "",
                    ""
                    ));
            groupC.Items.Add(new SampleDataItem("Clothing Store",
                    "Clothing Store",
                    "clothing_store",
                    "Assets/clothing store.png",
                    "",
                    " "
                    ));
            //groupC.Items.Add(new SampleDataItem("Coffee Shop",
            //        "Coffee Shop",
            //        "",
            //        "Assets/coffee shop.png",
            //        "",
            //        ""
            //        ));
            //groupC.Items.Add(new SampleDataItem("Cemetry",
            //        "Cemetry",
            //        "",
            //        "Assets/construction.png",
            //        "",
            //        ""
            //        ));
            //groupC.Items.Add(new SampleDataItem("Convenience Store",
            //        "Convenience Store",
            //        "convenience_store",
            //        "Assets/convenience store.png",
            //        "",
            //        ""
            //        ));
            //groupC.Items.Add(new SampleDataItem("Court House",
            //        "Court House",
            //        "",
            //        "Assets/court house.png",
            //        "",
            //        ""
            //        ));

            this._groups.Add(groupC);

            var groupD = new SampleDataGroup("Group-D",
                   "D",
                   "",
                   "",

                   "");

            groupD.Items.Add(new SampleDataItem("Dentist",
                    "Dentist",
                    "dentist",
                    "Assets/Dentist.png",
                    "",
                    " "
                    ));
            groupD.Items.Add(new SampleDataItem("Department Store",
                    "Department Store",
                    "department_store",
                    "Assets/department store.png",
                    "",
                    ""
                    ));
            groupD.Items.Add(new SampleDataItem("Doctor",
                    "Doctor",
                    "health",
                    "Assets/doctor.png",
                    "",
                    " "
                    ));

            this._groups.Add(groupD);

            var groupE = new SampleDataGroup("Group-E",
                        "E",
                        "",
                        "",

                        "");

            groupE.Items.Add(new SampleDataItem("Electronics Store",
                    "Electronics Store",
                    "electronics_store",
                    "Assets/electrician.png",
                    "",
                    " "
                    ));

            this._groups.Add(groupE);





            var groupF = new SampleDataGroup("Group-F",
                    "F",
                    "",
                    "",

                    "");

            groupF.Items.Add(new SampleDataItem("Finance",
                   "Finance",
                   "finance",
                   "Assets/Finance.png",
                   "",
                   " "
                   ));

            //groupF.Items.Add(new SampleDataItem("Funeral Home",
            //        "Funeral Home",
            //        "",
            //        "Assets/funeral home.png",
            //        "",
            //        " "
            //        ));

            groupF.Items.Add(new SampleDataItem("Fast Food",
                    "Fast Food",
                    "food",
                    "Assets/fastfood.png",
                    "",
                    ""
                    ));

            groupF.Items.Add(new SampleDataItem("Furniture Store",
                    "Furniture Store",
                    "furniture_store",
                    "Assets/furniture.png",
                    "",
                    ""
                    ));


            this._groups.Add(groupF);


            var groupG = new SampleDataGroup("Group-G",
                        "G",
                        "",
                        "",

                        "");

            groupG.Items.Add(new SampleDataItem("Gym",
                    "Gym",
                    "gym",
                    "Assets/gym.png",
                    "",
                    ""
                    ));



            groupG.Items.Add(new SampleDataItem("Gas Station",
                    "Gas Station",
                    "gas_station",
                    "Assets/gas station.png",
                    "",
                    ""
                    ));

            this._groups.Add(groupG);

            var groupH = new SampleDataGroup("Group-H",
                        "H",
                        "Countries starting with H",
                        "",

                        "");

            //groupH.Items.Add(new SampleDataItem("Hair Dressers",
            //        "Hair Dressers",
            //        "",
            //        "Assets/hand dresses.png",
            //        "",
            //        ""
            //        ));

            groupH.Items.Add(new SampleDataItem("Hospital",
                    "Hospital",
                    "hospital",
                    "Assets/hospital.png",
                    "",
                    ""
                    ));


            this._groups.Add(groupH);


            var groupI = new SampleDataGroup("Group-I",
                        "I",
                        "",
                        "",

                        "");

            groupI.Items.Add(new SampleDataItem("Insurance Agency",
                    "Insurance Agency",
                    "insurance_agency",
                    "Assets/Insurance.png",
                    "",
                    ""
                    ));

            this._groups.Add(groupI);



            //var groupJ = new SampleDataGroup("Group-J",
            //            "J",
            //            "",

            //            "Assets/groupF.png",
            //            "");

            //groupJ.Items.Add(new SampleDataItem("Jewellery Store",
            //        "Jewellery Store",
            //        "",
            //        "Assets/jewelerystore.png",
            //        "",
            //        ""
            //        ));

            // this._groups.Add(groupJ);





            var groupL = new SampleDataGroup("Group-L",
                            "L",
                            "",
                            "",

                            "");

            groupL.Items.Add(new SampleDataItem("Laundry",
                    "Laundry",
                    "laundry",
                    "Assets/laundry.png",
                   "",
                    ""
                    ));
            groupL.Items.Add(new SampleDataItem("Lawyer",
                   "Lawyer",
                   "lawyer",
                   "Assets/lawyer.png",
                   "",
                   ""
                   ));
            groupL.Items.Add(new SampleDataItem("Library",
                   "Library",
                   "library",
                   "Assets/Library.png",
                   "",
                   ""
                   ));
            groupL.Items.Add(new SampleDataItem("Local Government Office",
                   "Local Government Office",
                   "local_government_office",
                   "Assets/localgovtoffice.png",
                   "",
                   ""
                   ));
            //groupL.Items.Add(new SampleDataItem("Lock Smith",
            //       "Lock Smith",
            //       "",
            //       "Assets/lock smith.png",
            //       "",
            //       ""
            //       ));
            groupL.Items.Add(new SampleDataItem("Lodging",
                   "Lodging",
                   "lodging",
                   "Assets/lodging.png",
                    "",
                   ""
                   ));

            this._groups.Add(groupL);



            var groupM = new SampleDataGroup("Group-M",
                        "M",
                        "",
                        "",

                        "");

            groupM.Items.Add(new SampleDataItem("Meal Delivery",
                    "Meal Delivery",
                     "meal_delivery",
                    "Assets/meal delivery.png",
                   "",
                   " "
                    ));

            groupM.Items.Add(new SampleDataItem("Museum",
                   "Museum",
                   "museum",
                   "Assets/Museum.png",
                   "",
                   ""
                    ));

            this._groups.Add(groupM);



            var groupN = new SampleDataGroup("Group-N",
                        "N",
                        "",
                        "",

                        "");

            groupN.Items.Add(new SampleDataItem("Night Club",
                    "Night Club",
                    "night_club",
                    "Assets/nigt club.png",
                    "",
                    ""
                    ));

            this._groups.Add(groupN);




            var groupP = new SampleDataGroup("Group-P",
                    "P",
                    "",
                    "",

                    "");

            groupP.Items.Add(new SampleDataItem("Painter",
                    "Painter",
                    "painter",
                    "Assets/Painter.png",
                    " ",
                    ""
                    ));

            groupP.Items.Add(new SampleDataItem("Parking",
                    "Parking",
                    "parking",
                    "Assets/Parking.png",
                    "",
                    ""
                    ));
            groupP.Items.Add(new SampleDataItem("Pet Store",
                    "Pet Store",
                    "pet_store",
                    "Assets/pet store.png",
                   "",
                    ""
                    ));
            groupP.Items.Add(new SampleDataItem("Pharmacy",
                    "Pharmacy",
                    "pharmacy",
                    "Assets/pharmacy.png",
                   "",
                    ""
                    ));
            groupP.Items.Add(new SampleDataItem("Police Station",
                    "Police",
                    "police",
                    "Assets/Police.png",
                   "",
                    ""
                    ));
            groupP.Items.Add(new SampleDataItem("Post Office",
                    "Post Office",
                    "post_office",
                    "Assets/post Office.png",
                   "",
                    ""
                    ));

            this._groups.Add(groupP);




            var groupR = new SampleDataGroup("Group-R",
                      "R",
                      "",
                      "",

                      "");

            groupR.Items.Add(new SampleDataItem("Restaurant",
                    "Restaurant",
                    "restaurant",
                    "Assets/Restaurant.png",
                    "",
                    ""
                    ));
            //groupR.Items.Add(new SampleDataItem("Railway Station",
            //        "Railway Station",
            //        "",
            //        "Assets/railway station.png",
            //       "",
            //        ""
            //        ));

            this._groups.Add(groupR);


            var groupS = new SampleDataGroup("Group-S",
                        "S",
                        "",
                        "",

                        "");

            groupS.Items.Add(new SampleDataItem("School",
                    "School",
                    "school",
                    "Assets/school.png",
                    "",
                    ""
                    ));

            //groupS.Items.Add(new SampleDataItem("Shopping Centres",
            //       "Shopping Centres",
            //       "",
            //       "Assets/Shopping centres.png",
            //      "",
            //      ""
            //        ));
            //groupS.Items.Add(new SampleDataItem("Super Market",
            //        "Super Market",
            //        "",
            //        "Assets/super Market.png",
            //        "",
            //        ""
            //        ));
            groupS.Items.Add(new SampleDataItem("Stadium",
                    "Stadium",
                    "stadium",
                    "Assets/stadium.png",
                   "",
                   ""
                    ));

            this._groups.Add(groupS);


            //var groupT = new SampleDataGroup("Group-T",
            //                "T",
            //                "",
            //                "",

            //                "");

            //groupT.Items.Add(new SampleDataItem("Taxi",
            //        "Taxi",
            //        "",
            //        "Assets/taxi.png",
            //       "",
            //        ""
            //        ));

            // this._groups.Add(groupT);



            var groupV = new SampleDataGroup("Group-V",
                    "V",
                    "",
                    "",

                    "");

            groupV.Items.Add(new SampleDataItem("Veterinary Care",
                    "Veterinary Care",
                    "veterinary_care",
                    "Assets/veterinary care.png",
                    "",
                    ""

                    ));

            this._groups.Add(groupV);




        }


    }
}
