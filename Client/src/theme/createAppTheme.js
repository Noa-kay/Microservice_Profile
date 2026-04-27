import { alpha, createTheme } from '@mui/material/styles';

function getPalette(mode) {
  const isDark = mode === 'dark';

  return {
    mode,
    // MUI palette utilities (lighten/darken/contrastText) require concrete color values.
    primary: {
      main: isDark ? '#4a4540' : '#4a4540',
      light: isDark ? '#625b54' : '#6b6660',
      dark: isDark ? '#37322e' : '#2f2b27',
      contrastText: isDark ? '#f6f0eb' : '#ffffff',
    },
    secondary: {
      main: isDark ? '#8c7f73' : '#ede8e1',
      light: isDark ? '#a79a8f' : '#f4efea',
      dark: isDark ? '#6f645a' : '#d8d1c8',
      contrastText: isDark ? '#f7f2ed' : '#4a4540',
    },
    background: {
      default: isDark ? '#1a1715' : '#f6f1ee',
      paper: isDark ? '#26211d' : '#ffffff',
    },
    text: {
      primary: isDark ? '#eee4db' : '#4a4540',
      secondary: isDark ? '#c5b6a7' : '#6b6660',
    },
    divider: isDark ? '#3d342d' : '#e8e4e0',
    ...(isDark && {
      action: {
        hover: alpha('#eee4db', 0.06),
        selected: alpha('#eee4db', 0.1),
        disabled: alpha('#eee4db', 0.38),
        disabledBackground: alpha('#eee4db', 0.12),
      },
    }),
  };
}

/**
 * כפתורים במצב בהיר — משפחה אחת: טקסט כמעט שחור על רקע חום-בהיר / קרם (בלי גוון כחול).
 */
const SAND_BTN = {
  text: '#141210',
  border: 'rgba(74, 60, 48, 0.24)',
  /** רקע בהיר (מסגרת / טאב לא פעיל) */
  surface: '#faf7f2',
  /** מילוי כפתור מלא — חום בהיר נוטה ללבן */
  filled: '#ebe3d8',
};

/** מעבר עכבר: חום מעט כהה יותר (לא כחול) */
const SAND_BTN_HOVER = '#dfd4c6';
const SAND_BTN_HOVER_BORDER = 'rgba(74, 60, 48, 0.38)';

export default function createAppTheme(mode) {
  const isDark = mode === 'dark';
  const base = createTheme({
    direction: 'rtl',
    palette: getPalette(mode),
    typography: {
      fontFamily: 'var(--font-family-base)',
      h1: {
        fontSize: 'var(--font-size-h1)',
        fontWeight: 700,
        lineHeight: 'var(--line-height-tight)',
        fontFamily: 'var(--font-family-heading)',
      },
      h6: {
        fontSize: 'var(--font-size-h6)',
        fontWeight: 600,
        lineHeight: 'var(--line-height-tight)',
      },
      body1: {
        fontSize: 'var(--font-size-body)',
        lineHeight: 'var(--line-height-base)',
      },
      body2: {
        fontSize: 'var(--font-size-body-sm)',
        lineHeight: 'var(--line-height-relaxed)',
      },
      button: {
        textTransform: 'none',
        fontWeight: 700,
        fontFamily: 'var(--font-family-base)',
      },
    },
    shape: { borderRadius: 16 },
    components: {
      MuiCssBaseline: {
        styleOverrides: {
          body: ({ theme }) => ({
            margin: 0,
            backgroundColor: theme.palette.background.default,
            color: theme.palette.text.primary,
          }),
        },
      },
      MuiToolbar: {
        styleOverrides: {
          root: { minHeight: 64 },
        },
      },
      MuiPaper: {
        styleOverrides: {
          root: ({ theme }) => ({
            backgroundImage: 'none',
            border: `1px solid ${theme.palette.divider}`,
            ...(isDark && {
              backgroundColor: theme.palette.background.paper,
              boxShadow: `0 8px 24px ${alpha('#000000', 0.26)}`,
            }),
          }),
        },
      },
      MuiButton: {
        defaultProps: {
          disableElevation: true,
        },
        styleOverrides: {
          root: {
            borderRadius: 12,
            textTransform: 'none',
            fontWeight: 700,
            boxShadow: 'none',
            ...(isDark
              ? {}
              : {
                  color: SAND_BTN.text,
                  border: `1px solid ${SAND_BTN.border}`,
                  '& .MuiButton-startIcon, & .MuiButton-endIcon': {
                    color: 'inherit',
                  },
                }),
          },
          ...(isDark
            ? {
                contained: ({ theme }) => ({
                  backgroundColor: theme.palette.primary.main,
                  color: theme.palette.primary.contrastText,
                  '&:hover': {
                    backgroundColor: theme.palette.primary.light,
                  },
                  '&.Mui-disabled': {
                    backgroundColor: theme.palette.action.disabledBackground,
                    color: theme.palette.action.disabled,
                  },
                }),
                outlined: ({ theme }) => ({
                  borderColor: alpha(theme.palette.primary.light, 0.56),
                  color: theme.palette.primary.light,
                  '&:hover': {
                    borderColor: alpha(theme.palette.primary.light, 0.82),
                    backgroundColor: alpha(theme.palette.primary.light, 0.12),
                  },
                }),
              }
            : {
                contained: {
                  backgroundColor: SAND_BTN.filled,
                  color: SAND_BTN.text,
                  '&:hover': {
                    backgroundColor: SAND_BTN_HOVER,
                    boxShadow: 'none',
                  },
                  '&.Mui-disabled': {
                    backgroundColor: '#e8e4de',
                    color: 'rgba(20, 18, 16, 0.45)',
                    borderColor: SAND_BTN.border,
                  },
                },
                containedPrimary: {
                  backgroundColor: SAND_BTN.filled,
                  color: SAND_BTN.text,
                  '&:hover': {
                    backgroundColor: SAND_BTN_HOVER,
                    boxShadow: 'none',
                  },
                },
                containedSecondary: {
                  backgroundColor: SAND_BTN.filled,
                  color: SAND_BTN.text,
                  '&:hover': {
                    backgroundColor: SAND_BTN_HOVER,
                    boxShadow: 'none',
                  },
                },
                containedError: {
                  backgroundColor: SAND_BTN.filled,
                  color: SAND_BTN.text,
                  '&:hover': {
                    backgroundColor: SAND_BTN_HOVER,
                    boxShadow: 'none',
                  },
                },
                containedSuccess: {
                  backgroundColor: SAND_BTN.filled,
                  color: SAND_BTN.text,
                  '&:hover': {
                    backgroundColor: SAND_BTN_HOVER,
                    boxShadow: 'none',
                  },
                },
                outlined: {
                  backgroundColor: SAND_BTN.surface,
                  color: SAND_BTN.text,
                  border: `1px solid ${SAND_BTN.border}`,
                  '&:hover': {
                    backgroundColor: SAND_BTN_HOVER,
                    border: `1px solid ${SAND_BTN_HOVER_BORDER}`,
                    boxShadow: 'none',
                  },
                },
                outlinedPrimary: {
                  backgroundColor: SAND_BTN.surface,
                  color: SAND_BTN.text,
                  border: `1px solid ${SAND_BTN.border}`,
                  '&:hover': {
                    backgroundColor: SAND_BTN_HOVER,
                    border: `1px solid ${SAND_BTN_HOVER_BORDER}`,
                  },
                },
                outlinedInherit: {
                  backgroundColor: SAND_BTN.surface,
                  color: SAND_BTN.text,
                  border: `1px solid ${SAND_BTN.border}`,
                  '&:hover': {
                    backgroundColor: SAND_BTN_HOVER,
                    border: `1px solid ${SAND_BTN_HOVER_BORDER}`,
                  },
                },
                text: {
                  color: SAND_BTN.text,
                  border: '1px solid transparent',
                  '&:hover': {
                    backgroundColor: SAND_BTN_HOVER,
                  },
                },
                textPrimary: {
                  color: SAND_BTN.text,
                },
                textInherit: {
                  color: SAND_BTN.text,
                  border: '1px solid transparent',
                  '&:hover': {
                    backgroundColor: SAND_BTN_HOVER,
                  },
                },
              }),
        },
      },
      ...(isDark
        ? {}
        : {
            MuiIconButton: {
              styleOverrides: {
                root: {
                  color: SAND_BTN.text,
                  backgroundColor: SAND_BTN.filled,
                  border: `1px solid ${SAND_BTN.border}`,
                  boxShadow: 'none',
                  '&:hover': {
                    backgroundColor: SAND_BTN_HOVER,
                  },
                  '&.Mui-disabled': {
                    backgroundColor: '#e8e4de',
                    color: 'rgba(20, 18, 16, 0.4)',
                    borderColor: SAND_BTN.border,
                  },
                },
                colorPrimary: {
                  color: SAND_BTN.text,
                  backgroundColor: SAND_BTN.filled,
                  border: `1px solid ${SAND_BTN.border}`,
                  '&:hover': {
                    backgroundColor: SAND_BTN_HOVER,
                  },
                },
                colorInherit: {
                  color: SAND_BTN.text,
                  backgroundColor: SAND_BTN.surface,
                  border: `1px solid ${SAND_BTN.border}`,
                  '&:hover': {
                    backgroundColor: SAND_BTN_HOVER,
                  },
                },
              },
            },
            MuiChip: {
              styleOverrides: {
                root: {
                  fontWeight: 600,
                  color: SAND_BTN.text,
                  border: `1px solid ${SAND_BTN.border}`,
                  backgroundColor: SAND_BTN.surface,
                  '&:hover': {
                    backgroundColor: SAND_BTN_HOVER,
                  },
                },
                outlined: {
                  backgroundColor: SAND_BTN.surface,
                  color: SAND_BTN.text,
                  border: `1px solid ${SAND_BTN.border}`,
                },
                colorPrimary: {
                  backgroundColor: SAND_BTN.surface,
                  color: SAND_BTN.text,
                  border: `1px solid ${SAND_BTN.border}`,
                },
                deleteIcon: {
                  color: SAND_BTN.text,
                  '&:hover': {
                    color: SAND_BTN.text,
                  },
                },
              },
            },
          }),
      MuiCard: {
        styleOverrides: {
          root: ({ theme }) => ({
            borderRadius: 24,
            ...(isDark && {
              backgroundColor: alpha(theme.palette.background.paper, 0.92),
              border: `1px solid ${alpha(theme.palette.divider, 0.9)}`,
              boxShadow: `0 10px 30px ${alpha('#000000', 0.3)}`,
            }),
          }),
        },
      },
      MuiDrawer: {
        styleOverrides: {
          paper: ({ theme }) => ({
            ...(isDark && {
              backgroundColor: '#201b18',
              borderLeft: `1px solid ${alpha(theme.palette.divider, 0.9)}`,
            }),
          }),
        },
      },
      MuiAppBar: {
        defaultProps: { elevation: 0, color: 'transparent' },
        styleOverrides: {
          root: ({ theme }) => ({
            ...(isDark && {
              backgroundColor: alpha(theme.palette.background.default, 0.8),
              backdropFilter: 'blur(6px)',
              borderBottom: `1px solid ${alpha(theme.palette.divider, 0.7)}`,
            }),
          }),
        },
      },
      MuiLinearProgress: {
        styleOverrides: {
          root: {
            height: 3,
            borderRadius: 999,
          },
        },
      },
    },
  });

  const shadows = [...base.shadows];
  shadows[1] = 'var(--shadow-md)';

  return createTheme({ ...base, shadows });
}

