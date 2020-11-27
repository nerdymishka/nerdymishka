/**
 * Implement Gatsby's SSR (Server Side Rendering) APIs in this file.
 *
 * See: https://www.gatsbyjs.com/docs/ssr-apis/
 */

// You can delete this file if you're not using it

  
import React from "react"
import { oneLine, stripIndent } from "common-tags"

export function onRenderBody ({setHeadComponents, setPreBodyComponents, reporter}, pluginOptions) {
    var head = [];
    var body = [];

    body.push(
        <noscript 
           key="gtm-noscript"
           dangerouslySetInnerHTML={{
               __html: `<!-- Google Tag Manager (noscript) -->
               <noscript><iframe src="https://www.googletagmanager.com/ns.html?id=UA-140803820-1"
               height="0" width="0" style="display:none;visibility:hidden"></iframe></noscript>
               <!-- End Google Tag Manager (noscript) -->`
           }} />
    )

    
    head.push(
        <script key="gtm-load" async src="https://www.googletagmanager.com/gtag/js?id=UA-140803820-1"></script>
    )
    head.push(
        <script key="gtm"
          dangerouslySetInnerHTML={{
              __html: `
              window.dataLayer = window.dataLayer || [];
              function gtag(){dataLayer.push(arguments);}
              gtag('js', new Date());
            
              gtag('config', 'UA-140803820-1');
              `,
          }}

          />
    )

    if(head && head.length)
    {
        setHeadComponents(head);
    }

    if(body && body.length)
    {
        setPreBodyComponents(body);
    }
}