import React from "react";
import CookieConsent, { Cookies } from "react-cookie-consent";
import { Container } from "reactstrap";

export default function Footer(props)  {
    return (
        <footer style={{ marginTop: `2rem` }}>
            <Container>
                © {new Date().getFullYear()}, Built with {` `}
                <a href="https://www.gatsbyjs.com">Gatsby</a>
            </Container>
        <CookieConsent
            location="bottom"
            buttonText="Accept"
            declineButtonText="Decline"
            onDecline={() => {
                Cookies.remove("_ga");
            }}
            onAccept={() => {
                (window.dataLayer = window.dataLayer || []).push({ "event": "cookieAllowed" });
                Cookies.set("gdpr", {
                    "ga": true,
                    "ads": true,
                });

            }}
            cookieName="gdpr-cookies-allowed">
            This site uses cookies ...
        </CookieConsent>
    </footer>
    )
}