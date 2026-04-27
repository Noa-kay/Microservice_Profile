import { Box } from '@mui/material';
import Header from '../Header/Header.jsx';
import Footer from '../Footer/Footer.jsx';

function Layout({ children }) {
  return (
    <Box sx={{ display: 'flex', flexDirection: 'column', minHeight: '100vh' }}>
      <Header />
      <Box
        component="main"
        className="layout-main"
        sx={{
          flex: 1,
          width: '100%',
          maxWidth: 1200,
          mx: 'auto',
          px: { xs: 1.5, sm: 2.5 },
          py: { xs: 1.75, sm: 2.25 },
          boxSizing: 'border-box',
        }}
      >
        {children}
      </Box>
      <Footer />
    </Box>
  );
}

export default Layout;
