import { useState } from "react";
import { gql, useMutation } from "@apollo/client";


const CREATE_USER_MUTATION = gql`
  mutation CreateUser($entity: RegisterUserInput!) {
    registerUser(input: $entity) {
      user{
        id
        name
        password
        lastName
        email
        birthDate
      }
    }
  }
`;

export type UserInCreation = {
  email: string;
  mot_de_passe: string;
  prenom: string;
  nom: string;
  date_de_naissance: Date | string;
  sexe?: string | undefined;
};

export type User = UserInCreation & {
  id: string;
};

export const useInscription = (): [
  (newUser: UserInCreation) => Promise<void>,
  User | undefined,
  string | undefined,
  boolean
] => {
  const [data, setData] = useState<User | undefined>();
  const [error, setError] = useState<string | undefined>();
  const [loading, setLoading] = useState<boolean>(false);

  const [createUserMutation] = useMutation(CREATE_USER_MUTATION);

  async function inscription(newUser: UserInCreation): Promise<void> {
    setData(undefined);
    setError(undefined);
    setLoading(true);

    try {
      const response = await createUserMutation({
        variables: {
          entity: {
            name: newUser.prenom,
            lastname: newUser.nom,
            email: newUser.email,
            password: newUser.mot_de_passe,
            birthDate: newUser.date_de_naissance,
          },
        },
      });

      
      const user = response.data.create;
      //console.log(user, response);
      setData(user);
    } catch (err) {
      let message = "Erreur lors de l'inscription";
      if (err instanceof Error) {
        message = err.message;
      }
      setError(message);
      console.error(err);
    } finally {
      setLoading(false);
    }
  }

  return [inscription, data, error, loading];
};