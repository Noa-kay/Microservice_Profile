import { Box, Container, Paper } from '@mui/material';

const pageShellPaperSx = {
  borderRadius: '16px',
  px: { xs: 2, sm: 2.75 },
  py: { xs: 2, sm: 2.25 },
  bgcolor: 'background.paper',
  '& .MuiTypography-h1': { fontSize: { xs: '1.75rem', sm: '1.9rem' }, lineHeight: 1.2 },
  '& .MuiTypography-h4': { fontSize: { xs: '1.5rem', sm: '1.625rem' }, lineHeight: 1.25 },
  '& .MuiTypography-h5': { fontSize: { xs: '1.25rem', sm: '1.35rem' }, lineHeight: 1.3 },
  '& .MuiTypography-h6': { fontSize: '1.0625rem', lineHeight: 1.3 },
  '& .MuiTypography-body1': { fontSize: '0.9375rem', lineHeight: 1.55 },
  '& .MuiTypography-body2': { fontSize: '0.8125rem', lineHeight: 1.5 },
};

export default function PageShell({ children }) {
  return (
    <Box sx={{ backgroundColor: 'background.default', py: { xs: 0.5, sm: 1 } }}>
      <Container maxWidth="lg" sx={{ px: { xs: 0, sm: 1 } }}>
        <Paper elevation={0} sx={pageShellPaperSx}>
          {children}
        </Paper>
      </Container>
    </Box>
  );
}

