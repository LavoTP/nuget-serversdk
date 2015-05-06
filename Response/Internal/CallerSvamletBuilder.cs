using System;
using System.Linq;
using Sinch.Callback.Model;
using Sinch.Callback.Request;
using Sinch.Callback.Request.Internal;

namespace Sinch.Callback.Response.Internal
{
    internal class CallerSvamletBuilder : SvamletBuilder
    {
        internal CallerSvamletBuilder(Locale locale)
            : base(locale)
        {
        }

        public ISvamletResponse ConnectPstn(string destination, TimeSpan timeout, string callerId = null, bool suppressCallbacks = false)
        {
            if(timeout.TotalMinutes > 240)
                throw new BuilderException("Cannot specify more than 4 hours of calling");

            if (!string.IsNullOrEmpty(destination))
            {
                if (!destination.StartsWith("+"))
                    throw new BuilderException("Phone number should start with a '+'");

                if (destination.Length < 7)
                    throw new BuilderException("Phone number too short");

                if (destination.Length > 17)
                    throw new BuilderException("Phone number too long");

                if (destination.Substring(1).Any(c => !char.IsDigit(c)))
                    throw new BuilderException("Phone numbers should only have digits after '+'");
            }

            SetAction(new SvamletAction
            {
                Name = "connectpstn",
                DialTimeout = (int) timeout.TotalSeconds,
                Cli = callerId ?? "private",
                Locale = Locale.Code,
                Destination = new IdentityModel
                {
                    Type = "number",
                    Endpoint = destination
                },
                SuppressCallbacks = suppressCallbacks
            });

            return Build();
        }

        public ISvamletResponse ConnectMxp(IIdentity destination, string callerId = null)
        {
            if (destination == null || string.IsNullOrEmpty(destination.Endpoint))
                throw new BuilderException("No destionation given");

            if (destination.Endpoint.Length > 128)
                throw new BuilderException("Destination too long");

            IdentityModel destinationModel;

            if(!TypeMapper.Singleton.TryConvert(destination, out destinationModel))
                throw new BuilderException("Cannot interpres destination");

            SetAction(new SvamletAction
            {
                Name = "connectmxp",
                Cli = callerId ?? "private",
                Locale = Locale.Code,
                Destination = destinationModel
            });

            return Build();
        }

        public ISvamletResponse ConnectMxp(string userName, string callerId = null)
        {
            if (!string.IsNullOrEmpty(userName))
            {
                if (userName.Length > 128)
                    throw new BuilderException("User name too long");
            }

            SetAction(new SvamletAction
            {
                Name = "connectmxp",
                Cli = callerId ?? "private",
                Locale = Locale.Code,
                Destination = new IdentityModel
                {
                    Type = "username",
                    Endpoint = userName
                }

            });

            return Build();
        }

        public ISvamletResponse ConnectConference(string conferenceId)
        {
            if (string.IsNullOrEmpty(conferenceId))
                throw new BuilderException("Conference id must be supplied");
            
            if(conferenceId.Length > 128)
                throw new BuilderException("Conference id too long (max 128 characters)");

            SetAction(new SvamletAction
            {
                Name = "connectconf",
                ConferenceId = conferenceId,
                Locale = Locale.Code
            });

            return Build();
        }

        public ISvamletResponse Park(string holdPrompt, TimeSpan timeout)
        {
            if (string.IsNullOrEmpty(holdPrompt))
                throw new BuilderException("A hold prompt must be supplied");

            if (timeout.TotalSeconds < 60 && timeout.TotalSeconds > 600)
                throw new BuilderException("The timeout must be between 1 and 10 minutes");

            SetAction(new SvamletAction
            {
                Name = "park",
                HoldPrompt = holdPrompt,
                Locale = Locale.Code,
                DialTimeout = (int) timeout.TotalSeconds
            });

            return Build();
        }
    }
}