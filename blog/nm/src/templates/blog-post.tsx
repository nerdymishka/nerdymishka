import React from "react"
import { Link, graphql } from "gatsby"
import Layout from "../components/layout"
import Seo from "../components/seo"
import { MDXRenderer } from "gatsby-plugin-mdx"
import { mdx } from "@mdx-js/react/dist/esm"
import MdxBody from "../components/mdx-body"

interface BlogPostProps 
{
    
}

export default function BlogPostTemplate({ data, location }) {
  const post = data.mdx 
  const siteTitle = data.site.siteMetadata?.title || `Title`
  const { previous, next } = data
  const title = post.frontmatter.title || siteTitle
  const description = post.frontmatter.description || post.excerpt 
  return (
    <Layout>
      <Seo
        title={title}
        description={description}
      />
      <article
        className="blog-post"
        itemScope
        itemType="http://schema.org/Article"
      >
        <header>
        
          <p>{post.frontmatter.date}</p>
        </header>
        <h1 itemProp="headline">{post.frontmatter.title}</h1>
        <time>{post.frontmatter.date}</time>
        <section>
                <MdxBody>{post.body}</MdxBody>
        </section>
        <hr />
        <footer>
        </footer>
      </article>
      <nav className="blog-post-nav">
        <ul
          style={{
            display: `flex`,
            flexWrap: `wrap`,
            justifyContent: `space-between`,
            listStyle: `none`,
            padding: 0,
          }}
        >
          <li>
            {previous && (
              <Link to={previous.fields.slug} rel="prev">
                ← {previous.frontmatter.title}
              </Link>
            )}
          </li>
          <li>
            {next && (
              <Link to={next.fields.slug} rel="next">
                {next.frontmatter.title} →
              </Link>
            )}
          </li>
        </ul>
      </nav>
    </Layout>
  )
}

export const pageQuery = graphql`
  query BlogPostBySlug(
    $id: String!
    $previousPostId: String
    $nextPostId: String
  ) {
    site {
      siteMetadata {
        title
      }
    }
    mdx(id: { eq: $id }) {
      id
      excerpt(pruneLength: 160)
      html
      frontmatter {
        title
        date
        description
      }
    }
    previous: mdx(id: { eq: $previousPostId }) {
      fields {
        slug
      }
      frontmatter {
        title
      }
    }
    next: mdx(id: { eq: $nextPostId }) {
      fields {
        slug
      }
      frontmatter {
        title
      }
    }
  }
`