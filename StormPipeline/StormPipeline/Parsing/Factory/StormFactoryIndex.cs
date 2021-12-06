namespace Com.Apdcomms.StormPipeline.Parsing.Factory
{
    using System.Collections.Generic;

    public class StormFactoryIndex
    {
        public static readonly Dictionary<MessageCodes, MessageClass> messageAbbrievations = new Dictionary<MessageCodes, MessageClass>
        {
            { MessageCodes.PRI, MessageClass.ProcessResourceInformation },
            { MessageCodes.CI2, MessageClass.CreateIncidentTwo },
            { MessageCodes.DI, MessageClass.DeleteIncident  },
            { MessageCodes.SDA, MessageClass.SetDataAttribute },
            { MessageCodes.UI, MessageClass.UpdateIncident },
            { MessageCodes.UR, MessageClass.UpdateResource },
            { MessageCodes.PSI, MessageClass.ProcessStationInformation },
            { MessageCodes.CI, MessageClass.CreateIncident },
            { MessageCodes.CR, MessageClass.CreateResource },
            { MessageCodes.DCO, MessageClass.CreateDynamicObject },
            { MessageCodes.DDO, MessageClass.DeleteDynamicObject },
            { MessageCodes.CR2, MessageClass.CreateResourceTwo },
            { MessageCodes.RSC, MessageClass.ResourceShiftChange },
            { MessageCodes.RFS, MessageClass.RemoveFireStation },
            { MessageCodes.PII, MessageClass.ProcessIncidentInformation },
            { MessageCodes.PILI, MessageClass.ProcessIncidentLogInformation },
            { MessageCodes.DIL, MessageClass.DeleteIncidentLog },
            { MessageCodes.RILA, MessageClass.RequestIncidentLogAddition },
            { MessageCodes.DR, MessageClass.DeleteResource },
            { MessageCodes.RRA, MessageClass.RequestResourceAssociation },
            { MessageCodes.RRS, MessageClass.RequestResourceStatus},
            { MessageCodes.AL, MessageClass.VehicleAlert},
            { MessageCodes.CSU, MessageClass.ChangeStatusUpdate},
            { MessageCodes.CLI, MessageClass.CloseIncident},
            { MessageCodes.FWA, MessageClass.SendWatchAlert},
            { MessageCodes.CAI, MessageClass.CreateAssociatedIcon},
            { MessageCodes.ZAI, MessageClass.ZoomAssociatedIcon},
            { MessageCodes.DAI, MessageClass.DeleteAssociatedIcon},
        };
    }
}
