/**
 * Implement Gatsby's Browser APIs in this file.
 *
 * See: https://www.gatsbyjs.com/docs/browser-apis/
 */

// You can delete this file if you're not using it
import CookieConsent, { Cookies } from 'react-cookie-consent';
import ReactGA from 'react-ga';

export const onClientEntry = (_, pluginOptions = {}) => {
    var e = process.env.NODE_ENV;
    var isDev = e === "dev" || e === "development";
    var data = Cookies.get("gdpr");
    console.log("routeUpdate");
    console.log(data)
    console.log()
    if(data !== null)
    {
        data = JSON.parse(data);
        console.log("data");
        if(data.ga === true)
        {
            console.log("ga");
            console.log("init-ga");
            var dataLayer = (window.dataLayer = window.dataLayer || []);
            console.log(dataLayer)
            
            function gtag(){dataLayer.push(arguments);}
            gtag("js", new Date());
            gtag("config", "UA-140803820-1");
     
            ReactGA.initialize("UA-140803820-1", {
                titleCase: false,
                gaOptions: {
                    cookieName: "nm-google-analytics"
                },
                redactEmail: false,
                testMode: isDev ? true : false,
                debug: isDev ? true : false,
                alwaysSendToDefaultTracker: true,
            });
        }

        if(data.ads)
        {
            
        }
    }
}
  

export const onRouteUpdate = ({ location }, pluginOptions = {}) => {
    
    var data = Cookies.get("gdpr");
    
    if(data !== null)
    {
        data = JSON.parse(data);
        if(data.ads)
        {
            ReactGA.set({ page: window.location.pathname, anonymizeIp: false, allowAdFeatures: true });
            ReactGA.pageview(window.location.pathname + window.location.search);
        }
    }
}