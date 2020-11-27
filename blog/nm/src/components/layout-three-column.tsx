import React, { Children, useState } from "react"
import PropTypes, { node } from "prop-types"
import { useStaticQuery, graphql } from "gatsby"
import "./../scss/theme.scss"
import { faWindowAlt } from "@fortawesome/pro-duotone-svg-icons";
import { querySiteData } from "../queries/query-site-data";
import Header from "./header";
import { Container } from "react-bootstrap";
import Footer from "./footer";
import { Col } from "reactstrap";

declare global {
    interface Window {
        dataLayer: Array<any>
    }
}

export interface ThreeColumnLayoutProps 
{
    children?: React.ReactNode
    widgets?: React.ReactNode
    nav?: React.ReactNode
    data?: any 
}


export default function LayoutThreeColumn (props: ThreeColumnLayoutProps) {
    const { title, description } = querySiteData()

    return (
        <>
            <Header />
            <div className="main wrapper">
                <Container>
                    <div className="col-left col-3">
                        {props.nav}
                    </div>
                    <main className="col-6">
                        {props.children}
                    </main>
                    <div className="widgets col-right col-3">
                        {props.widgets}
                    </div>
                </Container>
            </div>
            <Footer />
        </>
    )
}

LayoutThreeColumn.propTypes = {
    children: PropTypes.node.isRequired,
}
