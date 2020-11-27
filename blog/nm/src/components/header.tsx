import React, { Children, useState } from "react"
import PropTypes, { node } from "prop-types"
import { useStaticQuery, graphql } from "gatsby"
import ReactGA from 'react-ga';
import CookieConsent, { Cookies } from 'react-cookie-consent';
import { querySiteData } from "../data/query-site-data";

import {
    Collapse,
    Navbar,
    Container,
    NavbarToggler,
    NavbarBrand,
    Nav,
    NavItem,
    NavLink,
    UncontrolledDropdown,
    DropdownToggle,
    DropdownMenu,
    DropdownItem,
    NavbarText
} from 'reactstrap';

const Header = ({}) => {
  const { title, description } = querySiteData();
  const [isOpen, setIsOpen] = useState(false);
  const toggle = () => setIsOpen(!isOpen);
   return (
      <header>
      <Navbar color="light" light expand="md">
          <Container>
              <NavbarBrand href="/">reactstrap</NavbarBrand>
              <NavbarToggler onClick={toggle} />
              <Collapse isOpen={isOpen} navbar>
                  <Nav className="mr-auto" navbar>
                      <NavItem>
                          <NavLink href="/components/">Components</NavLink>
                      </NavItem>
                      <NavItem>
                          <NavLink href="https://github.com/reactstrap/reactstrap">GitHub</NavLink>
                      </NavItem>
                      <UncontrolledDropdown nav inNavbar>
                          <DropdownToggle nav caret>
                              Options
                          </DropdownToggle>
                          <DropdownMenu right>
                              <DropdownItem>
                                  Option 1
                              </DropdownItem>
                              <DropdownItem>
                                  Option 2
                              </DropdownItem>
                              <DropdownItem divider />
                              <DropdownItem>
                                  Reset
                              </DropdownItem>
                          </DropdownMenu>
                      </UncontrolledDropdown>
                  </Nav>
                  <NavbarText>Simple Text</NavbarText>
              </Collapse>
          </Container>
      </Navbar>
  </header>
   );
}

export default Header
