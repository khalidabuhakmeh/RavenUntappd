using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Scraper.Models
{
    public class Checkin
    {
        public string Id { get; set; }
        [JsonProperty("beer_name")]
        public string BeerName { get; set; }
        [JsonProperty("brewery_name")]
        public string BreweryName { get; set; }
        [JsonProperty("beer_type")]
        public string BeerType { get; set; }
        [JsonProperty("beer_abv")]
        public decimal BeerAbv { get; set; }
        [JsonProperty("beer_ibu")]
        public decimal BeerIbu { get; set; }
        [JsonProperty("comment")]
        public string Comment { get; set; }
        [JsonProperty("venue_name")]
        public string VenueName { get; set; }
        [JsonProperty("venue_city")]
        public string VenueCity { get; set; }
        [JsonProperty("venue_state")]
        public string VenueState { get; set; }
        [JsonProperty("venue_lat")]
        public decimal? VenueLatitude { get; set; }
        [JsonProperty("venue_lng")]
        public decimal? VenueLongitude { get; set; }
        [JsonProperty("rating_score")]
        public string RatingScore { get; set; }
        [JsonProperty("created_at")]
        public DateTimeOffset CreatedAt { get; set; }
        [JsonProperty("checkin_url")]
        public string CheckinUrl { get; set; }
        [JsonProperty("beer_url")]
        public string BeerUrl { get; set; }
        [JsonProperty("brewery_url")]
        public string BreweryUrl { get; set; }
        [JsonProperty("brewery_country")]
        public string BreweryCountry { get; set; }

        public string BreweryId
        {
            get { return BreweryUrl.Replace("https://untappd.com/", ""); }
        }
    }

    public class Brewery
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string RawLocation { get; set; }
        public string Style { get; set; }

        public Brewery ProcessLocation(string location)
        {
            if (string.IsNullOrWhiteSpace(location))
                return this;

            RawLocation = location;
            try
            {
                City = location.Substring(0, location.IndexOf(',')).Trim();
                var stateAndCountry = location.Replace(City, "").Replace(",", "").Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);

                State = stateAndCountry.Select(x => x.Trim()).FirstOrDefault();
                Country = string.Join(" ", stateAndCountry.Skip(1));
            }
            catch (Exception e)
            {
                Console.WriteLine(location);
            }
            return this;
        }
    }
}
