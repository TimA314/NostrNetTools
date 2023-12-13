using System.Text.Json.Serialization;

namespace NostrNetTools.Nostr.NIP11
{
    public class NostrRelayMetadata
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("pubkey")]
        public string? Pubkey { get; set; }

        [JsonPropertyName("contact")]
        public string? Contact { get; set; }

        [JsonPropertyName("supported_nips")]
        public List<int>? SupportedNips { get; set; }

        [JsonPropertyName("software")]
        public string? Software { get; set; }

        [JsonPropertyName("version")]
        public string? Version { get; set; }

        [JsonPropertyName("icon")]
        public string? Icon { get; set; }

        [JsonPropertyName("limitation")]
        public RelayLimitations? Limitation { get; set; }

        [JsonPropertyName("retention")]
        public List<RelayRetention>? Retention { get; set; }

        [JsonPropertyName("relayCountries")]
        public List<string>? RelayCountries { get; set; }

        [JsonPropertyName("preferences")]
        public CommunityPreferences? Preferences { get; set; }

        [JsonPropertyName("paymentsInfo")]
        public PayToRelayInfo? PaymentsInfo { get; set; }

        public class RelayLimitations
        {
            [JsonPropertyName("maxMessageLength")]
            public int? MaxMessageLength { get; set; }

            [JsonPropertyName("maxSubscriptions")]
            public int? MaxSubscriptions { get; set; }

            [JsonPropertyName("maxFilters")]
            public int? MaxFilters { get; set; }

            [JsonPropertyName("maxLimit")]
            public int? MaxLimit { get; set; }

            [JsonPropertyName("maxSubidLength")]
            public int? MaxSubidLength { get; set; }

            [JsonPropertyName("maxEventTags")]
            public int? MaxEventTags { get; set; }

            [JsonPropertyName("maxContentLength")]
            public int? MaxContentLength { get; set; }

            [JsonPropertyName("minPowDifficulty")]
            public int? MinPowDifficulty { get; set; }

            [JsonPropertyName("authRequired")]
            public bool? AuthRequired { get; set; }

            [JsonPropertyName("paymentRequired")]
            public bool? PaymentRequired { get; set; }

            [JsonPropertyName("restrictedWrites")]
            public bool? RestrictedWrites { get; set; }

            [JsonPropertyName("createdAtLowerLimit")]
            public int? CreatedAtLowerLimit { get; set; }

            [JsonPropertyName("createdAtUpperLimit")]
            public int? CreatedAtUpperLimit { get; set; }
        }

        public class RelayRetention
        {
            [JsonPropertyName("kinds")]
            public List<int>? Kinds { get; set; }

            [JsonPropertyName("time")]
            public int? Time { get; set; }

            [JsonPropertyName("count")]
            public int? Count { get; set; }
        }

        public class CommunityPreferences
        {
            [JsonPropertyName("languageTags")]
            public List<string>? LanguageTags { get; set; }

            [JsonPropertyName("tags")]
            public List<string>? Tags { get; set; }

            [JsonPropertyName("postingPolicy")]
            public string? PostingPolicy { get; set; }
        }

        public class PayToRelayInfo
        {
            [JsonPropertyName("paymentsUrl")]
            public string? PaymentsUrl { get; set; }

            [JsonPropertyName("fees")]
            public FeesStructure? Fees { get; set; }

            public class FeesStructure
            {
                [JsonPropertyName("admission")]
                public List<FeeInfo>? Admission { get; set; }

                [JsonPropertyName("subscription")]
                public List<FeeInfo>? Subscription { get; set; }

                [JsonPropertyName("publication")]
                public List<FeeInfo>? Publication { get; set; }

                public class FeeInfo
                {
                    [JsonPropertyName("amount")]
                    public long? Amount { get; set; }

                    [JsonPropertyName("unit")]
                    public string? Unit { get; set; }

                    [JsonPropertyName("period")]
                    public int? Period { get; set; }

                    [JsonPropertyName("kinds")]
                    public List<int>? Kinds { get; set; }
                }
            }
        }
    }

}
