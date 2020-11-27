/**
 * Layout component that queries for data
 * with Gatsby's useStaticQuery component
 *
 * See: https://www.gatsbyjs.com/docs/use-static-query/
 */

import React, { Children, useState } from "react"
import PropTypes, { node } from "prop-types"
import { useStaticQuery, graphql } from "gatsby"
import "./../scss/theme.scss"
import { faWindowAlt } from "@fortawesome/pro-duotone-svg-icons";
import { querySiteData } from "../queries/query-site-data";
import Header from "./header";
import { Container } from "react-bootstrap";
import Footer from "./footer";

declare global {
    interface Window {
        dataLayer: Array<any>
    }
}

export interface LayoutProps 
{
    children?: React.ReactNode
    widgets?: React.ReactNode
    data?: any 
}


export default function LayoutTwoColumn (props: LayoutProps) {
    const { title, description } = querySiteData()

    return (
        <>
            <Header />
            <div className="main wrapper">
                <Container>
                    <main className="col-8">
                        {props.children}
                    </main>
                    <div className="col-4">
                        {props.widgets}
                    </div>
                </Container>
            </div>
            <Footer />
        </>
    )
}

LayoutTwoColumn.propTypes = {
    children: PropTypes.node.isRequired,
}
