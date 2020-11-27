/**
 * SEO component that queries for data with
 *  Gatsby's useStaticQuery React hook
 *
 * See: https://www.gatsbyjs.com/docs/use-static-query/
 */

import React, { Fragment } from "react"
import PropTypes from "prop-types"
import { Helmet  } from "react-helmet"
import { useStaticQuery, graphql } from "gatsby"
import { JsxElement } from "typescript"

interface SeoProps {
   title?: string 
   description?: string
   lang?: string 
   meta?: Array<any>
   keywords?: string 
}

export default function Seo ({title, description, lang, meta, keywords}: SeoProps) {
  const { site } = useStaticQuery(
    graphql`
      query {
        site {
          siteMetadata {
            title
            description
            author
          }
        }
      }
    `
  )
  const metaDescription = description || site.siteMetadata.description
  const defaultTitle = site.siteMetadata?.title
  const metaAttributes = [{

  }].concat(meta)
  return (
    <Helmet 
      title={defaultTitle} 
      meta={[
        { name: "description", content: description },
        { name: "keywords", content: keywords },
      ]}
      />

  )
}

Seo.defaultProps = {
  lang: `en`,
  meta: [],
  description: ``,
}

Seo.propTypes = {
  description: PropTypes.string,
  lang: PropTypes.string,
  meta: PropTypes.arrayOf(PropTypes.object),
  title: PropTypes.string.isRequired,
}