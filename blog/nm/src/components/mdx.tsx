import { MDXProvider } from "@mdx-js/react/dist/esm"
import { MdxComponents } from "./mdx-components"

export default function Mdx({ children }) {

    return (
        <MDXProvider components={MdxComponents}>{children}</MDXProvider>
    )
}