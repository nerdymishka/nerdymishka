import React from "react"
import { graphql } from "gatsby"
import Layout from "../components/layout"
var formatDate = function(value) {
    if(!value)
      return "";
    var dt = new Date(value);
    return `${dt.getMonth() + 1}/${dt.getDate()}/${dt.getFullYear()}`
  }
export default class BlogList extends React.Component<{data: any}> {
  render() {
    const posts = this.props.data.allMdx.edges
    return (
      <Layout>
        <>
            <Layout key="index">
            {posts.map(({ excerpt, frontmatter }, index) => (
                
                <article key={index}>
                    <h1>{frontmatter.title}</h1>
                    <time dateTime={frontmatter.date}>{formatDate(frontmatter.date)}</time>
                    <p>{excerpt}</p>
                </article>
                ))}
            </Layout>
        </>
      </Layout>
    )
  }
}
export const blogListQuery = graphql`
query blogListQuery($skip: Int!, $limit: Int!) {
    site {
        siteMetadata {
          title
          description
          author
        }
    }
    allMdx(
        sort: { fields: [frontmatter___date], order: DESC }
        filter: { frontmatter: { status: { eq: "published" } } }
        limit: $limit,
        skip: $skip
    ) {
        edges {
            node {
                excerpt(pruneLength: 300)
                id
                frontmatter {
                    title
                    date
                    slug
                    categories
                    tags
                    status
                    keywords
                }
            }
        }
    }    
}
`