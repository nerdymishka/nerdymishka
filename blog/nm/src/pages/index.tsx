import React, { Fragment } from "react"
import { graphql, Link } from "gatsby"

import Layout from "../components/layout"
import Image from "../components/image"
import SEO from "../components/seo"

var formatDate = function(value) {
  if(!value)
    return "";
  var dt = new Date(value);
  return `${dt.getMonth() + 1}/${dt.getDate()}/${dt.getFullYear()}`
}

export default ({data}) => {
  const posts = data.allMdx.edges.map(o => o.node);
  return (
    <>
      <Layout key="index"  >
      {posts.map(({ excerpt, frontmatter }, index) => (
        
          <article key={index}>
            <h1>{frontmatter.title}</h1>
            <time dateTime={frontmatter.date}>{formatDate(frontmatter.date)}</time>
            <p>{excerpt}</p>
          </article>
        ))}
        <Fragment key="widgets">
           <h2>sidebar</h2>
        </Fragment>
      </Layout>
    </>
  )
};

export const query = graphql`
query blogIndexQuery   {
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
      limit: 10
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
`;
