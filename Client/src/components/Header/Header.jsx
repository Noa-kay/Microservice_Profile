import { useState } from 'react';
import { AppBar, Toolbar, Typography, Button, Box, IconButton } from '@mui/material';
import MenuIcon from '@mui/icons-material/Menu';
import { Link as RouterLink, useLocation, useNavigate } from 'react-router-dom';
import { useAuth } from '../../context/AuthContext';

const navLinks = [
  { label: 'דף הבית', path: '/' },
  { label: 'משרות', path: '/jobs' },
  { label: 'פרופילים', path: '/profiles' },
  { label: 'אודות', path: '/about' },
  { label: 'צור קשר', path: '/contact' },
  { label: 'הגדרות', path: '/settings' },
];

function Header() {
  const [mobileOpen, setMobileOpen] = useState(false);
  const { pathname } = useLocation();
  const navigate = useNavigate();
  const { isAuthenticated, user, logout, isLoading } = useAuth();

  const handleLogout = () => {
    logout();
    setMobileOpen(false);
    navigate('/', { replace: true });
  };

  return (
    <AppBar
      position="sticky"
      sx={{
        bgcolor: 'transparent',
        borderBottom: '1px solid',
        borderColor: 'divider',
        backdropFilter: 'blur(8px)',
      }}
    >
      <Toolbar
        sx={{
          maxWidth: 1200,
          mx: 'auto',
          width: '100%',
          minHeight: 64,
          px: { xs: 2, sm: 3 },
        }}
      >
        <Typography
          variant="h6"
          component={RouterLink}
          to="/"
          sx={{
            fontWeight: 600,
            textDecoration: 'none',
            flexShrink: 0,
            color: 'text.primary',
            fontFamily: 'var(--font-family-heading)',
          }}
        >
          השמה לסמינר
        </Typography>

        <Box component="nav" sx={{ display: { xs: 'none', md: 'flex' }, gap: 1, mr: 4 }}>
          {navLinks.map(({ label, path }) => (
            <Button
              key={path}
              component={RouterLink}
              to={path}
              color="inherit"
              variant={pathname === path ? 'outlined' : 'text'}
              sx={{ minWidth: 'auto', px: 1.5, color: 'text.primary' }}
            >
              {label}
            </Button>
          ))}
        </Box>

        <Box
          sx={{
            display: { xs: 'none', md: 'flex' },
            alignItems: 'center',
            gap: 1.5,
            mr: 'auto',
            flexWrap: 'wrap',
            justifyContent: 'flex-end',
          }}
        >
          {!isLoading && isAuthenticated && (
            <>
              <Typography
                variant="body2"
                color="text.secondary"
                sx={{
                  maxWidth: 220,
                  overflow: 'hidden',
                  textOverflow: 'ellipsis',
                  whiteSpace: 'nowrap',
                }}
                title={user?.email || ''}
              >
                {user?.email ? `מחובר: ${user.email}` : 'מחובר'}
              </Typography>
              <Button onClick={handleLogout} size="small" variant="outlined" color="inherit">
                לוגאאוט
              </Button>
            </>
          )}
          {!isLoading && !isAuthenticated && (
            <>
              <Button component={RouterLink} to="/login" size="small" variant="outlined">
                התחברות
              </Button>
              <Button component={RouterLink} to="/signup" size="small" variant="contained">
                הרשמה
              </Button>
            </>
          )}
        </Box>

        <IconButton
          onClick={() => setMobileOpen(!mobileOpen)}
          aria-label="תפריט"
          sx={{ display: { xs: 'inline-flex', md: 'none' }, mr: 'auto' }}
        >
          <MenuIcon />
        </IconButton>
      </Toolbar>

      {mobileOpen && (
        <Box
          sx={{
            display: { xs: 'flex', md: 'none' },
            flexDirection: 'column',
            p: 2,
            backgroundColor: 'background.paper',
            borderTop: '1px solid',
            borderColor: 'divider',
          }}
        >
          {navLinks.map(({ label, path }) => (
            <Button
              key={path}
              component={RouterLink}
              to={path}
              onClick={() => setMobileOpen(false)}
              fullWidth
              variant={pathname === path ? 'outlined' : 'text'}
              sx={{ justifyContent: 'flex-end', py: 1.5, color: 'text.primary' }}
            >
              {label}
            </Button>
          ))}
          {!isLoading && isAuthenticated && (
            <>
              <Typography
                variant="body2"
                color="text.secondary"
                sx={{ alignSelf: 'flex-end', mt: 1, px: 1 }}
              >
                {user?.email ? `מחובר: ${user.email}` : 'מחובר'}
              </Typography>
              <Button
                onClick={handleLogout}
                fullWidth
                variant="outlined"
                sx={{ justifyContent: 'flex-end', py: 1.5, mt: 1 }}
              >
                לוגאאוט
              </Button>
            </>
          )}
          {!isLoading && !isAuthenticated && (
            <>
              <Button
                component={RouterLink}
                to="/login"
                onClick={() => setMobileOpen(false)}
                fullWidth
                variant="outlined"
                sx={{ justifyContent: 'flex-end', py: 1.5, mt: 1 }}
              >
                התחברות
              </Button>
              <Button
                component={RouterLink}
                to="/signup"
                onClick={() => setMobileOpen(false)}
                fullWidth
                variant="contained"
                sx={{ justifyContent: 'flex-end', py: 1.5, mt: 1 }}
              >
                הרשמה
              </Button>
            </>
          )}
        </Box>
      )}
    </AppBar>
  );
}

export default Header;
