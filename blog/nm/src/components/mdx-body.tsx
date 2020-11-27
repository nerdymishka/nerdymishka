import React from "react";
import { MDXRenderer } from "gatsby-plugin-mdx"
import { MdxComponents } from "./mdx-components"


export default function MdxBody({children}) {
    return (
        <MDXRenderer components={MdxComponents}>{children}</MDXRenderer>
    )
}