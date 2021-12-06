namespace Com.Apdcomms.DataGateway.LocationsService.Tpi
{
    using System;

    [Serializable]
    public record TpiLocationMetaData
    {
        /// <summary>
        /// The name of the resource
        /// </summary>
        public string Resource { get; init; }
        
        /// <summary>
        /// The class of the resource
        /// </summary>
        public string Class { get; init; }
        
        /// <summary>
        /// The date and time of the update.
        /// In Coordinator this was the system short date/time format of the server running AVLS.
        /// In Artemis this is the time format configured in the AVLS server's registry.
        /// </summary>
        public string Timestamp { get; init; }
        
        /// <summary>
        /// 2 digit hex value
        /// Bitmask showing the information that has been updated in the message.
        /// </summary>
        public int? Contents { get; init; }
        
        /// <summary>
        /// 2 digit hex value
        /// The event number of the last INCA or XDR event to be sent by the resource.
        /// </summary>
        public int? EventId { get; init; }
        
        /// <summary>
        /// The event description of the last INCA or XDR event to be sent by the resource.
        /// </summary>
        public string EventText { get; init; }
        
        /// <summary>
        /// 1 digit decimal
        /// The current fix status where 0 = NoFix, 1 = InFix, 2 = DifferentialFix(INCA1)/AssumedFix(XDR and INCA2), 3 = DeadReckoning
        /// </summary>
        public int? Fix { get; init; }
        
        /// <summary>
        /// Latitude on the WSG84 ellipsoid in decimal degrees. 
        /// </summary>
        public double? Latitude { get; init; }
        
        /// <summary>
        /// Longitude on the WSG84 ellipsoid in decimal degrees.
        /// </summary>
        public double? Longitude { get; init; }

        /// <summary>
        /// Speed in kilometers per hour.
        /// </summary>
        public double? Speed { get; init; }
        
        /// <summary>
        /// Direction of travel in degrees relative to truth north using a WSG84 frame of reference.
        /// </summary>
        public double? Bearing { get; init; }
        
        /// <summary>
        /// 1 digit decimal
        /// A number from - to 8 corresponding to the vehicle's status.
        /// </summary>
        public int? Status { get; init; }
        
        /// <summary>
        /// 4 digit hex
        /// A bitmask showing the current input status of the resource.
        /// </summary>
        public int? Inputs { get; init; }
        
        /// <summary>
        /// 2 digit hex
        /// A bitmask showing the current output status of the resource.
        /// </summary>
        public int? Outputs { get; init; }
        
        /// <summary>
        /// 2 digit hex
        /// A bitmask showing the currently active geofence classes on the resource. 
        /// </summary>
        public int? GeoData { get; init; }
        
        /// <summary>
        /// Last resource data message received. This data string will either be blank, contain an ISSI or contain a Driver ID.
        /// Driver ID's can only be sent in ARTEMIS DVMS systems, which do not currently support TETRA.
        /// A driver ID received in this field will be prepended with the string: [#!ONE_BOX]
        /// </summary>
        public string Data { get; init; }
    }
}